use byteorder::{NativeEndian, ReadBytesExt, WriteBytesExt};
use libz_sys;
use std::ffi::{c_int, c_ulong};
use std::fs::File;
use std::io::{Read, Write};
use std::mem::MaybeUninit;
use thiserror::Error as TeError;

#[derive(Debug, TeError)]
pub enum Error {
    #[error("given index is out of range")]
    IndexOutOfRange,
    #[error("arithmetic overflow")]
    NumOverflow,
    #[error("fail to cast numeric value")]
    BadNumCast,

    #[error("fail to read or write file")]
    Io(#[from] std::io::Error),
    #[error("fail to call zlib function")]
    ZlibCall,
    #[error("given TAS file is wrong")]
    BadTasFile,
}

type Result<T> = std::result::Result<T, Error>;

#[derive(Debug, Clone, Copy, PartialEq, Eq, PartialOrd, Ord, Hash)]
pub enum TasKey {
    KeyUp,
    KeyDown,
    KeyLeft,
    KeyRight,
    KeyShift,
    KeySpace,
    KeyQ,
    KeyEsc,
    KeyEnter,
}

#[derive(Debug, Clone, Copy, PartialEq, PartialOrd)]
#[repr(C)]
pub struct TasFrame {
    delta_time: f32,
    key_flags: u32,
}

impl TasFrame {
    pub fn new(delta_time: f32, key_flags: u32) -> Self {
        Self {
            delta_time,
            key_flags,
        }
    }

    pub fn with_delta_time(delta_time: f32) -> Self {
        Self::new(delta_time, 0u32)
    }
}

impl TasFrame {
    pub fn get_delta_time(&self) -> f32 {
        self.delta_time
    }

    pub fn set_delta_time(&mut self, delta_time: f32) -> () {
        self.delta_time = delta_time
    }
}

impl TasFrame {
    fn get_key_flag(key: TasKey) -> u32 {
        let bit = match key {
            TasKey::KeyUp => 0,
            TasKey::KeyDown => 1,
            TasKey::KeyLeft => 2,
            TasKey::KeyRight => 3,
            TasKey::KeyShift => 4,
            TasKey::KeySpace => 5,
            TasKey::KeyQ => 6,
            TasKey::KeyEsc => 7,
            TasKey::KeyEnter => 8,
        };
        1u32 << bit
    }

    pub fn is_key_pressed(&self, key: TasKey) -> bool {
        (self.key_flags & Self::get_key_flag(key)) != 0u32
    }

    pub fn set_key_pressed(&mut self, key: TasKey, pressed: bool) {
        if pressed {
            self.key_flags |= Self::get_key_flag(key)
        } else {
            self.key_flags &= !(Self::get_key_flag(key))
        }
    }

    pub fn flip_key_pressed(&mut self, key: TasKey) {
        self.key_flags ^= Self::get_key_flag(key)
    }

    pub fn clear_key_pressed(&mut self) {
        self.key_flags = 0
    }
}

#[derive(Debug)]
pub struct TasFile {
    frames: Vec<TasFrame>,
}

impl TasFile {
    pub fn new(frames: Vec<TasFrame>) -> Self {
        Self { frames }
    }

    pub fn load(filename: &str) -> Result<Self> {
        // Open file
        let mut reader = File::open(filename)?;

        // Read decompressed size.
        let u32_decomp_size = reader.read_u32::<NativeEndian>()?;
        let usize_decomp_size = usize::try_from(u32_decomp_size).map_err(|_| Error::BadNumCast)?;
        let culong_decomp_size =
            c_ulong::try_from(usize_decomp_size).map_err(|_| Error::BadNumCast)?;

        // Check size and compute frame count.
        let frame_size = size_of::<TasFrame>();
        let frame_count = usize_decomp_size
            .checked_div(frame_size)
            .ok_or(Error::NumOverflow)?;
        if !usize_decomp_size.is_multiple_of(frame_size) {
            return Err(Error::BadTasFile);
        }

        // Read all rest file into memory
        let mut comp_buffer = Vec::new();
        reader.read_to_end(&mut comp_buffer)?;
        // Get compressed buffer size.
        let usize_comp_size = comp_buffer.len();
        let culong_comp_size = c_ulong::try_from(usize_comp_size).map_err(|_| Error::BadNumCast)?;

        // Create decompressed buffer with uninitialized memory
        let mut decomp_buffer: Box<[MaybeUninit<TasFrame>]> = Box::new_uninit_slice(frame_count);

        // Decompress data
        let source = comp_buffer.as_ptr() as *const libz_sys::Bytef;
        let source_len = culong_comp_size;
        let dest = decomp_buffer.as_mut_ptr() as *mut libz_sys::Bytef;
        let mut dest_len = culong_decomp_size;
        let rv = unsafe { libz_sys::uncompress(dest, &mut dest_len, source, source_len) };
        if rv != libz_sys::Z_OK {
            return Err(Error::ZlibCall);
        }

        // Convert uninitialized buffer to initialized
        // SAFETY: We've just initialized the buffer with uncompress
        let frames = unsafe {
            std::slice::from_raw_parts(decomp_buffer.as_ptr() as *const TasFrame, frame_count)
                .to_vec()
        };

        // Okey
        Ok(Self::new(frames))
    }

    pub fn save(&self, filename: &str) -> Result<()> {
        // Open file
        let mut writer = File::create(filename)?;

        // Get decompressed size.
        let usize_decomp_size = size_of::<TasFrame>()
            .checked_mul(self.frames.len())
            .ok_or(Error::NumOverflow)?;

        // Write decompressed size.
        let u32_decomp_size = u32::try_from(usize_decomp_size).map_err(|_| Error::BadNumCast)?;
        writer.write_u32::<NativeEndian>(u32_decomp_size)?;

        // Get compressed buffer boundary
        let culong_decomp_size =
            c_ulong::try_from(usize_decomp_size).map_err(|_| Error::BadNumCast)?;
        let culong_comp_bound_size: c_ulong =
            unsafe { libz_sys::compressBound(culong_decomp_size) };

        // Create buffer for it.
        let usize_comp_bound_size =
            usize::try_from(culong_comp_bound_size).map_err(|_| Error::BadNumCast)?;
        let mut buffer: Box<[MaybeUninit<u8>]> = Box::new_uninit_slice(usize_comp_bound_size);

        // Compress data into buffer
        let source = self.frames.as_ptr() as *const libz_sys::Bytef;
        let source_len: c_ulong = culong_decomp_size;
        let dest = buffer.as_mut_ptr() as *mut libz_sys::Bytef;
        let mut dest_len: c_ulong = culong_comp_bound_size;
        let level: c_int = 9;
        let rv = unsafe { libz_sys::compress2(dest, &mut dest_len, source, source_len, level) };
        if rv != libz_sys::Z_OK {
            return Err(Error::ZlibCall);
        }

        // Fetch the final compressed length.
        let culong_comp_size: c_ulong = dest_len;
        let usize_comp_size: usize =
            usize::try_from(culong_comp_size).map_err(|_| Error::BadNumCast)?;

        // Write compressed data
        let buffer = unsafe { buffer.assume_init() };
        writer.write(&buffer[..usize_comp_size])?;

        // Okey
        Ok(())
    }
}

impl TasFile {
    /// 清空存储结构。
    pub fn clear(&mut self) {
        self.frames.clear()
    }

    /// 获取当前存储的TAS帧的个数。
    pub fn get_count(&self) -> usize {
        self.frames.len()
    }

    /// 获取当前存储结构是不是空的。
    pub fn is_empty(&self) -> bool {
        self.frames.is_empty()
    }
}

impl TasFile {
    fn check_index(&self, index: usize) -> bool {
        index < self.frames.len()
    }

    fn check_index_range(&self, index_from: usize, index_to: usize) -> bool {
        // Check index relation
        if index_to < index_from {
            return false;
        }
        // Check index range
        if index_to >= self.frames.len() {
            return false;
        }
        // Okey
        return true;
    }

    /// 访问给定索引的帧。
    pub fn visit<'a>(&'a self, index: usize) -> Result<&'a TasFrame> {
        if self.check_index(index) {
            Ok(&self.frames[index])
        } else {
            Err(Error::IndexOutOfRange)
        }
    }

    /// 以可变形式访问给定索引的值。
    pub fn visit_mut<'a>(&'a mut self, index: usize) -> Result<&'a mut TasFrame> {
        if self.check_index(index) {
            Ok(&mut self.frames[index])
        } else {
            Err(Error::IndexOutOfRange)
        }
    }

    /// 访问给定索引范围内的帧。
    pub fn batchly_visit<'a>(&'a self, index_from: usize, index_to: usize) -> Result<&'a [TasFrame]> {
        if self.check_index_range(index_from, index_to) {
            Ok(&self.frames[index_from..=index_to])
        } else {
            Err(Error::IndexOutOfRange)
        }
    }

    /// 以可变形式访问给定索引范围内的帧。
    pub fn batchly_visit_mut<'a>(&'a mut self, index_from: usize, index_to: usize) -> Result<&'a mut[TasFrame]> {
        if self.check_index_range(index_from, index_to) {
            Ok(&mut self.frames[index_from..=index_to])
        } else {
            Err(Error::IndexOutOfRange)
        }
    }
}

impl TasFile {
    /// 在结尾继续添加给的的帧序列。
    ///
    /// `frames`为要插入的元素的切片。
    pub fn append(&mut self, frames: &[TasFrame]) {
        self.frames.extend_from_slice(frames)
    }

    /// 在给定的索引**之前**插入给定的帧序列。
    ///
    /// 按照此函数约定，如果要在头部插入数据，则可以通过指定0来实现。
    /// 然而对于在尾部插入数据，或在空的存储中插入数据，可以指定存储结构的长度来实现。
    /// 即指定最大Index + 1的值来实现。
    ///
    /// `index`为要在前方插入数据的元素的索引。`frames`为要插入的元素的切片。
    pub fn insert(&mut self, index: usize, frames: &[TasFrame]) -> Result<()> {
        if index > self.frames.len() {
            Err(Error::IndexOutOfRange)
        } else if index == self.frames.len() {
            // Insert at tail
            self.frames.extend_from_slice(frames);
            Ok(())
        } else {
            // Insert at middle or head
            self.frames.splice(index..index, frames.iter().copied());
            Ok(())
        }
    }

    /// 将给定范围内的帧移除。
    ///
    /// `index_from`为要开始移除的单元的索引。`index_to`为最后一个要移除的单元的索引。
    /// `index_from`和`index_to`指向的帧均会被删除（端点inclusive模式）。
    /// `index_to`不能小于`index_from`。
    /// `index_from`和`index_to`均不能超过最大索引。
    pub fn remove(&mut self, index_from: usize, index_to: usize) -> Result<()> {
        // Check index relation
        if index_to < index_from {
            return Err(Error::IndexOutOfRange);
        }
        // Check index range
        if index_to >= self.frames.len() {
            return Err(Error::IndexOutOfRange);
        }
        // Perform remove
        self.frames.drain(index_from..=index_to);
        Ok(())
    }
}

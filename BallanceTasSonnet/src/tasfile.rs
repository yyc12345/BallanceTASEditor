use crate::utils::fps_converter;
use thiserror::Error as TeError;

#[derive(Debug, TeError)]
pub enum Error {
    #[error("{0}")]
    BadFpsConv(#[from] fps_converter::Error),

    #[error("given index is out of range")]
    IndexOutOfRange,
    #[error("arithmetic overflow")]
    NumOverflow,
}

type Result<T> = std::result::Result<T, Error>;

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

    pub fn with_fps(fps: f32) -> Result<Self> {
        Ok(Self::new(fps_converter::to_delta(fps)?, 0u32))
    }
}

impl TasFrame {
    pub fn get_delta_time(&self) -> f32 {
        self.delta_time
    }

    pub fn set_delta_time(&mut self, delta_time: f32) {
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

    pub fn get_key_up_pressed(&self) -> bool {
        self.is_key_pressed(TasKey::KeyUp)
    }
    pub fn set_key_up_pressed(&mut self, pressed: bool) {
        self.set_key_pressed(TasKey::KeyUp, pressed)
    }
    pub fn get_key_down_pressed(&self) -> bool {
        self.is_key_pressed(TasKey::KeyDown)
    }
    pub fn set_key_down_pressed(&mut self, pressed: bool) {
        self.set_key_pressed(TasKey::KeyDown, pressed)
    }
    pub fn get_key_left_pressed(&self) -> bool {
        self.is_key_pressed(TasKey::KeyLeft)
    }
    pub fn set_key_left_pressed(&mut self, pressed: bool) {
        self.set_key_pressed(TasKey::KeyLeft, pressed)
    }
    pub fn get_key_right_pressed(&self) -> bool {
        self.is_key_pressed(TasKey::KeyRight)
    }
    pub fn set_key_right_pressed(&mut self, pressed: bool) {
        self.set_key_pressed(TasKey::KeyRight, pressed)
    }
    pub fn get_key_shift_pressed(&self) -> bool {
        self.is_key_pressed(TasKey::KeyShift)
    }
    pub fn set_key_shift_pressed(&mut self, pressed: bool) {
        self.set_key_pressed(TasKey::KeyShift, pressed)
    }
    pub fn get_key_space_pressed(&self) -> bool {
        self.is_key_pressed(TasKey::KeySpace)
    }
    pub fn set_key_space_pressed(&mut self, pressed: bool) {
        self.set_key_pressed(TasKey::KeySpace, pressed)
    }
    pub fn get_key_q_pressed(&self) -> bool {
        self.is_key_pressed(TasKey::KeyQ)
    }
    pub fn set_key_q_pressed(&mut self, pressed: bool) {
        self.set_key_pressed(TasKey::KeyQ, pressed)
    }
    pub fn get_key_esc_pressed(&self) -> bool {
        self.is_key_pressed(TasKey::KeyEsc)
    }
    pub fn set_key_esc_pressed(&mut self, pressed: bool) {
        self.set_key_pressed(TasKey::KeyEsc, pressed)
    }
    pub fn get_key_enter_pressed(&self) -> bool {
        self.is_key_pressed(TasKey::KeyEnter)
    }
    pub fn set_key_enter_pressed(&mut self, pressed: bool) {
        self.set_key_pressed(TasKey::KeyEnter, pressed)
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

    pub fn from_brandnew(count: usize, frame: TasFrame) -> Self {
        Self::new(vec![frame; count])
    }

    pub fn from_brandnew_with_fps(count: usize, fps: f32) -> Result<Self> {
        Ok(Self::from_brandnew(count, TasFrame::with_fps(fps)?))
    }

    pub fn load_file(reader: dyn)

    pub fn from_file(file: &str) -> Result<Self> {
        todo!()
    }

    pub fn save(&self, file: &str) -> Result<Self> {
        todo!()
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
    /// 访问给定索引的帧。
    pub fn visit<'a>(&'a self, index: usize) -> Result<&'a TasFrame> {
        self.frames.get(index).ok_or(Error::IndexOutOfRange)
    }

    /// 以可变形式访问给定索引的值。
    pub fn visit_mut<'a>(&'a mut self, index: usize) -> Result<&'a mut TasFrame> {
        self.frames.get_mut(index).ok_or(Error::IndexOutOfRange)
    }

    /// 在给定的索引**之前**插入给定的项目。
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

    /// 从给定单元开始，移除给定个数的元素。
    ///
    /// `index`为要开始移除的单元的索引。`count`为要移除的元素的个数。
    pub fn remove(&mut self, index: usize, count: usize) -> Result<()> {
        // Check index
        let index_from = index;
        if index_from >= self.frames.len() {
            return Err(Error::IndexOutOfRange);
        }
        // Check count
        let count = if count == 0 {
            // Count == 0 may cause "..=" buggy, so we return first.
            return Ok(());
        } else {
            count - 1
        };
        let index_to = index.checked_add(count).ok_or(Error::NumOverflow)?;
        if index_to >= self.frames.len() {
            return Err(Error::IndexOutOfRange);
        }
        // Perform remove
        self.frames.drain(index_from..=index_to);
        Ok(())
    }
}

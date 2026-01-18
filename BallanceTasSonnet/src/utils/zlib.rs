use std::{io::{Read, Write}, mem::MaybeUninit};
use libz_sys;
use byteorder::{ReadBytesExt, WriteBytesExt, NativeEndian};
use thiserror::Error as TeError;
use crate::tasfile::TasFrame;

#[derive(Debug, TeError)]
pub enum Error {
    #[error("fail to read or write file")]
    Io(#[from] std::io::Error),
    #[error("arithmetic overflow")]
    NumOverflow,
    #[error("fail to cast numeric value")]
    BadNumCast,
}

type Result<T> = std::result::Result<T, Error>;

pub fn compress(writer: &mut dyn Write, frames: &[TasFrame]) -> Result<()> {
    // Get decompressed size.
    let usize_decomp_size = size_of::<TasFrame>().checked_mul(frames.len()).ok_or(Error::NumOverflow)?;

    // Write decompressed size.
    let u32_decomp_size = u32::try_from(usize_decomp_size).map_err(|e| Error::BadNumCast)?;
    writer.write_u32::<NativeEndian>(u32_decomp_size)?;

    let buffer: Box<[MaybeUninit<u8>]> = Box::new_uninit_slice(usize_decomp_size);
    

    Ok(())
}

pub fn decompress(reader: &mut dyn Read) -> Result<Vec<TasFrame>> {

}


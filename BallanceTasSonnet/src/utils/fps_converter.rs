use thiserror::Error as TeError;

#[derive(Debug, TeError)]
pub enum Error {
    #[error("delta time should not be zero or negative value")]
    BadDeltaTime,
    #[error("fps should should not be zero or negative value")]
    BadFps,
}

type Result<T> = std::result::Result<T, Error>;

pub fn to_fps(delta: f32) -> Result<f32> {
    if delta <= 0f32 {
        Err(Error::BadDeltaTime)
    } else {
        Ok(1f32 / delta)
    }
}

pub fn to_delta(fps: f32) -> Result<f32> {
    if fps <= 0f32 {
        Err(Error::BadFps)
    } else {
        Ok(1f32 / fps)
    }
}

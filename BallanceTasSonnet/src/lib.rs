use pyo3::prelude::*;

pub(crate) mod wrapped;

#[pymodule(name = "_blctas")]
/// Provides functionality for handling Ballance TAS works.
mod blctas {
    #[pymodule_export]
    use super::tasfile;
}

#[pymodule(submodule)]
/// Provides functionality for handling Ballance TAS files loading, saving and editing.
mod tasfile {
    use pyo3::{exceptions::PyRuntimeError, prelude::*};
    use crate::wrapped::tasfile::{
        Error as RsTasError, TasFile as RsTasFile, TasFrame as RsTasFrame, TasKey as RsTasKey
    };

    impl From<RsTasError> for PyErr {
        fn from(error: RsTasError) -> Self {
            PyRuntimeError::new_err(error.to_string())
        }
    }

    #[pyclass(eq, eq_int)]
    #[derive(Debug, Clone, Copy, PartialEq, Eq, PartialOrd, Ord, Hash)]
    /// Represents the different keys that can be pressed in a TAS frame.
    enum TasKey {
        #[pyo3(name = "KEY_UP")]
        /// Up arrow key
        KeyUp,
        #[pyo3(name = "KEY_DOWN")]
        /// Down arrow key
        KeyDown,
        #[pyo3(name = "KEY_LEFT")]
        /// Left arrow key
        KeyLeft,
        #[pyo3(name = "KEY_RIGHT")]
        /// Right arrow key
        KeyRight,
        #[pyo3(name = "KEY_SHIFT")]
        /// Shift key
        KeyShift,
        #[pyo3(name = "KEY_SPACE")]
        /// Spacebar key
        KeySpace,
        #[pyo3(name = "KEY_Q")]
        /// Q key
        KeyQ,
        #[pyo3(name = "KEY_ESC")]
        /// Escape key
        KeyEsc,
        #[pyo3(name = "KEY_ENTER")]
        /// Enter key
        KeyEnter,
    }

    impl From<TasKey> for RsTasKey {
        fn from(key: TasKey) -> RsTasKey {
            match key {
                TasKey::KeyUp => RsTasKey::KeyUp,
                TasKey::KeyDown => RsTasKey::KeyDown,
                TasKey::KeyLeft => RsTasKey::KeyLeft,
                TasKey::KeyRight => RsTasKey::KeyRight,
                TasKey::KeyShift => RsTasKey::KeyShift,
                TasKey::KeySpace => RsTasKey::KeySpace,
                TasKey::KeyQ => RsTasKey::KeyQ,
                TasKey::KeyEsc => RsTasKey::KeyEsc,
                TasKey::KeyEnter => RsTasKey::KeyEnter,
            }
        }
    }

    #[pyclass]
    #[derive(Debug)]
    /// Represents a TAS file containing a sequence of frames.
    /// Each frame contains key press information and delta time for Ballance gameplay.
    struct TasFile {
        inner: RsTasFile
    }

    #[pymethods]
    impl TasFile {
        // region: Status

        /// Clears all frames from the TAS file.
        fn clear(&mut self) -> PyResult<()> {
            Ok(self.inner.clear())
        }

        /// Gets the number of frames in the TAS file.
        /// 
        /// Returns:
        ///     The count of frames in the TAS file.
        fn get_count(&self) -> PyResult<usize> {
            Ok(self.inner.get_count())
        }

        /// Checks if the TAS file is empty (contains no frames).
        /// 
        /// Returns:
        ///     True if the TAS file is empty, False otherwise.
        fn is_empty(&self) -> PyResult<bool> {
            Ok(self.inner.is_empty())
        }

        // endregion

        // region: Single Operation

        /// Gets the delta time of a frame at the specified index.
        /// 
        /// Args:
        ///     index: The index of the frame to get the delta time from.
        /// 
        /// Returns:
        ///     The delta time value of the frame at the specified index.
        /// 
        /// Raises:
        ///     RuntimeError: If the index is out of range.
        fn get_delta_time(&self, index: usize) -> PyResult<f32> {
            Ok(self.inner.visit(index)?.get_delta_time())
        }

        /// Sets the delta time of a frame at the specified index.
        /// 
        /// Args:
        ///     index: The index of the frame to set the delta time for.
        ///     delta_time: The new delta time value to set.
        /// 
        /// Raises:
        ///     RuntimeError: If the index is out of range.
        fn set_delta_time(&mut self, index: usize, delta_time: f32) -> PyResult<()> {
            Ok(self.inner.visit_mut(index)?.set_delta_time(delta_time))
        }

        /// Checks if a specific key is pressed in the frame at the specified index.
        /// 
        /// Args:
        ///     index: The index of the frame to check.
        ///     key: The key to check for press status.
        /// 
        /// Returns:
        ///     True if the key is pressed, False otherwise.
        /// 
        /// Raises:
        ///     RuntimeError: If the index is out of range.
        fn is_key_pressed(&self, index: usize, key: TasKey) -> PyResult<bool> {
            Ok(self.inner.visit(index)?.is_key_pressed(key.into()))
        }

        /// Sets the press status of a specific key in the frame at the specified index.
        /// 
        /// Args:
        ///     index: The index of the frame to modify.
        ///     key: The key to set the press status for.
        ///     pressed: True to press the key, False to release it.
        /// 
        /// Raises:
        ///     RuntimeError: If the index is out of range.
        fn set_key_pressed(&mut self, index: usize, key: TasKey, pressed: bool) -> PyResult<()> {
            Ok(self.inner.visit_mut(index)?.set_key_pressed(key.into(), pressed))
        }

        /// Flips the press status of a specific key in the frame at the specified index.
        /// If the key was pressed, it becomes released; if it was released, it becomes pressed.
        /// 
        /// Args:
        ///     index: The index of the frame to modify.
        ///     key: The key to flip the press status for.
        /// 
        /// Raises:
        ///     RuntimeError: If the index is out of range.
        fn flip_key_pressed(&mut self, index: usize, key: TasKey) -> PyResult<()> {
            Ok(self.inner.visit_mut(index)?.flip_key_pressed(key.into()))
        }

        /// Clears all key presses in the frame at the specified index, setting all keys to released state.
        /// 
        /// Args:
        ///     index: The index of the frame to clear key presses for.
        /// 
        /// Raises:
        ///     RuntimeError: If the index is out of range.
        fn clear_key_pressed(&mut self, index: usize) -> PyResult<()> {
            Ok(self.inner.visit_mut(index)?.clear_key_pressed())
        }

        // endregion

        // region: Batchly Operation

        /// Sets the delta time for a range of frames from index_from to index_to (inclusive).
        /// This is a batch operation that modifies multiple frames in one call, which is more
        /// efficient than calling set_delta_time individually on each frame.
        /// 
        /// Args:
        ///     index_from: The starting index (inclusive) of the range to modify.
        ///     index_to: The ending index (inclusive) of the range to modify.
        ///     delta_time: The new delta time value to set for all frames in the range.
        /// 
        /// Raises:
        ///     RuntimeError: If either index is out of range or if index_to < index_from.
        fn batchly_set_delta_time(&mut self, index_from: usize, index_to: usize, delta_time: f32) -> PyResult<()> {
            for frame in self.inner.batchly_visit_mut(index_from, index_to)? {
                frame.set_delta_time(delta_time);
            }
            Ok(())
        }

        /// Sets the press status of a specific key for a range of frames from index_from to index_to (inclusive).
        /// This is a batch operation that modifies multiple frames in one call, which is more
        /// efficient than calling set_key_pressed individually on each frame.
        /// 
        /// Args:
        ///     index_from: The starting index (inclusive) of the range to modify.
        ///     index_to: The ending index (inclusive) of the range to modify.
        ///     key: The key to set the press status for.
        ///     pressed: True to press the key, False to release it for all frames in the range.
        /// 
        /// Raises:
        ///     RuntimeError: If either index is out of range or if index_to < index_from.
        fn batchly_set_key_pressed(&mut self, index_from: usize, index_to: usize, key: TasKey, pressed: bool) -> PyResult<()> {
            for frame in self.inner.batchly_visit_mut(index_from, index_to)? {
                frame.set_key_pressed(key.into(), pressed);
            }
            Ok(())
        }

        /// Flips the press status of a specific key for a range of frames from index_from to index_to (inclusive).
        /// This is a batch operation that modifies multiple frames in one call, which is more
        /// efficient than calling flip_key_pressed individually on each frame.
        /// 
        /// Args:
        ///     index_from: The starting index (inclusive) of the range to modify.
        ///     index_to: The ending index (inclusive) of the range to modify.
        ///     key: The key to flip the press status for in all frames in the range.
        /// 
        /// Raises:
        ///     RuntimeError: If either index is out of range or if index_to < index_from.
        fn batchly_flip_key_pressed(&mut self, index_from: usize, index_to: usize, key: TasKey) -> PyResult<()> {
            for frame in self.inner.batchly_visit_mut(index_from, index_to)? {
                frame.flip_key_pressed(key.into());
            }
            Ok(())
        }

        /// Clears all key presses for a range of frames from index_from to index_to (inclusive).
        /// This sets all keys to the released state for all frames in the range.
        /// This is a batch operation that modifies multiple frames in one call, which is more
        /// efficient than calling clear_key_pressed individually on each frame.
        /// 
        /// Args:
        ///     index_from: The starting index (inclusive) of the range to modify.
        ///     index_to: The ending index (inclusive) of the range to modify.
        /// 
        /// Raises:
        ///     RuntimeError: If either index is out of range or if index_to < index_from.
        fn batchly_clear_key_pressed(&mut self, index_from: usize, index_to: usize) -> PyResult<()> {
            for frame in self.inner.batchly_visit_mut(index_from, index_to)? {
                frame.clear_key_pressed();
            }
            Ok(())
        }

        // endregion

        // region: Modify

        /// Appends a specified number of frames with the given delta time to the end of the TAS file.
        /// 
        /// Args:
        ///     count: The number of frames to append.
        ///     delta_time: The delta time value for the new frames.
        fn append(&mut self, count: usize, delta_time: f32) -> PyResult<()> {
            let frames = vec![RsTasFrame::with_delta_time(delta_time); count];
            Ok(self.inner.append(&frames))
        }
        
        /// Inserts a specified number of frames with the given delta time at the specified index.
        /// 
        /// Args:
        ///     index: The position at which to insert the new frames.
        ///     count: The number of frames to insert.
        ///     delta_time: The delta time value for the new frames.
        /// 
        /// Raises:
        ///     RuntimeError: If the index is out of range.
        fn insert(&mut self, index: usize, count: usize, delta_time: f32) -> PyResult<()> {
            let frames = vec![RsTasFrame::with_delta_time(delta_time); count];
            Ok(self.inner.insert(index, &frames)?)
        }

        /// Removes frames from the TAS file within the specified range (inclusive).
        /// 
        /// Args:
        ///     index_from: The starting index (inclusive) of the range to remove.
        ///     index_to: The ending index (inclusive) of the range to remove.
        /// 
        /// Raises:
        ///     RuntimeError: If either index is out of range or if index_to < index_from.
        fn remove(&mut self, index_from: usize, index_to: usize) -> PyResult<()> {
            Ok(self.inner.remove(index_from, index_to)?)
        }

        // endregion
    }

    #[pyfunction]
    /// Creates a new TAS file with a specified number of frames, all having the same delta time.
    /// 
    /// Args:
    ///     count: The number of frames to create in the new TAS file.
    ///     delta_time: The delta time value for all frames in the new TAS file.
    /// 
    /// Returns:
    ///     A new TasFile instance with the specified number of frames.
    fn create(count: usize, delta_time: f32) -> PyResult<TasFile> {
        Ok(TasFile { inner: RsTasFile::new(vec![RsTasFrame::with_delta_time(delta_time); count]) })
    }

    #[pyfunction]
    /// Loads a TAS file from disk.
    /// 
    /// Args:
    ///     filename: The path to the TAS file to load.
    /// 
    /// Returns:
    ///     A TasFile instance loaded from the specified file.
    /// 
    /// Raises:
    ///     RuntimeError: If the file cannot be loaded or is invalid.
    fn load(filename: &str) -> PyResult<TasFile> {
        Ok(TasFile { inner: RsTasFile::load(filename)? })
    }

    #[pyfunction]
    /// Saves a TAS file to disk.
    /// 
    /// Args:
    ///     file: The TasFile instance to save.
    ///     filename: The path where the TAS file should be saved.
    /// 
    /// Raises:
    ///     RuntimeError: If the file cannot be saved.
    fn save(file: &TasFile, filename: &str) -> PyResult<()> {
        Ok(file.inner.save(filename)?)
    }

}

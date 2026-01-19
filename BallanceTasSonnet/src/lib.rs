use pyo3::prelude::*;

pub(crate) mod wrapped;

#[pymodule]
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
    enum TasKey {
        #[pyo3(name = "KEY_UP")]
        KeyUp,
        #[pyo3(name = "KEY_DOWN")]
        KeyDown,
        #[pyo3(name = "KEY_LEFT")]
        KeyLeft,
        #[pyo3(name = "KEY_RIGHT")]
        KeyRight,
        #[pyo3(name = "KEY_SHIFT")]
        KeyShift,
        #[pyo3(name = "KEY_SPACE")]
        KeySpace,
        #[pyo3(name = "KEY_Q")]
        KeyQ,
        #[pyo3(name = "KEY_ESC")]
        KeyEsc,
        #[pyo3(name = "KEY_ENTER")]
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
    struct TasFile {
        inner: RsTasFile
    }

    #[pymethods]
    impl TasFile {
        // region: Status

        fn clear(&mut self) -> PyResult<()> {
            Ok(self.inner.clear())
        }

        fn get_count(&self) -> PyResult<usize> {
            Ok(self.inner.get_count())
        }

        fn is_empty(&self) -> PyResult<bool> {
            Ok(self.inner.is_empty())
        }

        // endregion

        // region: Single Operation

        fn get_delta_time(&self, index: usize) -> PyResult<f32> {
            Ok(self.inner.visit(index)?.get_delta_time())
        }

        fn set_delta_time(&mut self, index: usize, delta_time: f32) -> PyResult<()> {
            Ok(self.inner.visit_mut(index)?.set_delta_time(delta_time))
        }

        fn is_key_pressed(&self, index: usize, key: TasKey) -> PyResult<bool> {
            Ok(self.inner.visit(index)?.is_key_pressed(key.into()))
        }

        fn set_key_pressed(&mut self, index: usize, key: TasKey, pressed: bool) -> PyResult<()> {
            Ok(self.inner.visit_mut(index)?.set_key_pressed(key.into(), pressed))
        }

        fn flip_key_pressed(&mut self, index: usize, key: TasKey) -> PyResult<()> {
            Ok(self.inner.visit_mut(index)?.flip_key_pressed(key.into()))
        }

        fn clear_key_pressed(&mut self, index: usize) -> PyResult<()> {
            Ok(self.inner.visit_mut(index)?.clear_key_pressed())
        }

        // endregion

        // region: Batchly Operation

        fn batchly_set_delta_time(&mut self, index_from: usize, index_to: usize, delta_time: f32) -> PyResult<()> {
            for frame in self.inner.batchly_visit_mut(index_from, index_to)? {
                frame.set_delta_time(delta_time);
            }
            Ok(())
        }

        fn batchly_set_key_pressed(&mut self, index_from: usize, index_to: usize, key: TasKey, pressed: bool) -> PyResult<()> {
            for frame in self.inner.batchly_visit_mut(index_from, index_to)? {
                frame.set_key_pressed(key.into(), pressed);
            }
            Ok(())
        }

        fn batchly_flip_key_pressed(&mut self, index_from: usize, index_to: usize, key: TasKey) -> PyResult<()> {
            for frame in self.inner.batchly_visit_mut(index_from, index_to)? {
                frame.flip_key_pressed(key.into());
            }
            Ok(())
        }

        fn batchly_clear_key_pressed(&mut self, index_from: usize, index_to: usize) -> PyResult<()> {
            for frame in self.inner.batchly_visit_mut(index_from, index_to)? {
                frame.clear_key_pressed();
            }
            Ok(())
        }

        // endregion

        // region: Modify

        fn append(&mut self, count: usize, delta_time: f32) -> PyResult<()> {
            let frames = vec![RsTasFrame::with_delta_time(delta_time); count];
            Ok(self.inner.append(&frames))
        }
        
        fn insert(&mut self, index: usize, count: usize, delta_time: f32) -> PyResult<()> {
            let frames = vec![RsTasFrame::with_delta_time(delta_time); count];
            Ok(self.inner.insert(index, &frames)?)
        }

        fn remove(&mut self, index_from: usize, index_to: usize) -> PyResult<()> {
            Ok(self.inner.remove(index_from, index_to)?)
        }

        // endregion
    }

    #[pyfunction]
    fn create(count: usize, delta_time: f32) -> PyResult<TasFile> {
        Ok(TasFile { inner: RsTasFile::new(vec![RsTasFrame::with_delta_time(delta_time); count]) })
    }

    #[pyfunction]
    fn load(filename: &str) -> PyResult<TasFile> {
        Ok(TasFile { inner: RsTasFile::load(filename)? })
    }

    #[pyfunction]
    fn save(file: &TasFile, filename: &str) -> PyResult<()> {
        Ok(file.inner.save(filename)?)
    }

}

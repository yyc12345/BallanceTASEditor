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

    // #[pyclass]
    // #[derive(Clone)]
    // pub struct PyTasFrame {
    //     inner: TasFrame,
    // }

    // // Constructor for PyTasFrame
    // impl PyTasFrame {
    //     fn new_from_inner(inner: TasFrame) -> Self {
    //         PyTasFrame { inner }
    //     }
    // }

    // #[pymethods]
    // impl PyTasFrame {
    //     #[new]
    //     fn new(delta_time: f32, key_flags: u32) -> PyTasFrame {
    //         PyTasFrame::new_from_inner(TasFrame::new(delta_time, key_flags))
    //     }

    //     #[staticmethod]
    //     fn with_fps(fps: f32) -> PyResult<PyTasFrame> {
    //         match TasFrame::with_fps(fps) {
    //             Ok(frame) => Ok(PyTasFrame::new_from_inner(frame)),
    //             Err(e) => Err(PyErr::new::<pyo3::exceptions::PyValueError, _>(
    //                 e.to_string(),
    //             )),
    //         }
    //     }

    //     #[getter]
    //     fn get_delta_time(&self) -> f32 {
    //         self.inner.get_delta_time()
    //     }

    //     fn get_fps(&self) -> PyResult<f32> {
    //         match self.inner.get_fps() {
    //             Ok(fps) => Ok(fps),
    //             Err(e) => Err(PyErr::new::<pyo3::exceptions::PyValueError, _>(
    //                 e.to_string(),
    //             )),
    //         }
    //     }

    //     fn set_delta_time(&mut self, delta_time: f32) {
    //         self.inner.set_delta_time(delta_time);
    //     }

    //     fn set_fps(&mut self, fps: f32) -> PyResult<()> {
    //         match self.inner.set_fps(fps) {
    //             Ok(()) => Ok(()),
    //             Err(e) => Err(PyErr::new::<pyo3::exceptions::PyValueError, _>(
    //                 e.to_string(),
    //             )),
    //         }
    //     }

    //     fn is_key_pressed(&self, key: PyTasKey) -> bool {
    //         self.inner.is_key_pressed(key.into())
    //     }

    //     fn set_key_pressed(&mut self, key: PyTasKey, pressed: bool) {
    //         self.inner.set_key_pressed(key.into(), pressed);
    //     }

    //     fn flip_key_pressed(&mut self, key: PyTasKey) {
    //         self.inner.flip_key_pressed(key.into());
    //     }

    //     fn clear_key_pressed(&mut self) {
    //         self.inner.clear_key_pressed();
    //     }
    // }

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

        fn get_fps(&self, index: usize) -> PyResult<f32> {
            Ok(self.inner.visit(index)?.get_fps()?)
        }

        fn set_delta_time(&mut self, index: usize, delta_time: f32) -> PyResult<()> {
            Ok(self.inner.visit_mut(index)?.set_delta_time(delta_time))
        }

        fn set_fps(&mut self, index: usize, fps: f32) -> PyResult<()> {
            Ok(self.inner.visit_mut(index)?.set_fps(fps)?)
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

        fn batchly_set_fps(&mut self, index_from: usize, index_to: usize, fps: f32) -> PyResult<()> {
            for frame in self.inner.batchly_visit_mut(index_from, index_to)? {
                frame.set_fps(fps)?;
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

        fn append_with_delta_time(&mut self, count: usize, delta_time: f32) -> PyResult<()> {
            let frames = vec![RsTasFrame::with_delta_time(delta_time); count];
            Ok(self.inner.append(&frames))
        }
        
        fn append_with_fps(&mut self, count: usize, fps: f32) -> PyResult<()> {
            let frames = vec![RsTasFrame::with_fps(fps)?; count];
            Ok(self.inner.append(&frames))
        }

        fn insert_with_delta_time(&mut self, index: usize, count: usize, delta_time: f32) -> PyResult<()> {
            let frames = vec![RsTasFrame::with_delta_time(delta_time); count];
            Ok(self.inner.insert(index, &frames)?)
        }

        fn insert_with_fps(&mut self, index: usize, count: usize, fps: f32) -> PyResult<()> {
            let frames = vec![RsTasFrame::with_fps(fps)?; count];
            Ok(self.inner.insert(index, &frames)?)
        }

        fn remove(&mut self, index_from: usize, index_to: usize) -> PyResult<()> {
            Ok(self.inner.remove(index_from, index_to)?)
        }

        // endregion
    }

    #[pyfunction]
    fn create_with_delta_time(count: usize, delta_time: f32) -> PyResult<TasFile> {
        Ok(TasFile { inner: RsTasFile::new(vec![RsTasFrame::with_delta_time(delta_time); count]) })
    }

    #[pyfunction]
    fn create_with_fps(count: usize, fps: f32) -> PyResult<TasFile> {
        Ok(TasFile { inner: RsTasFile::new(vec![RsTasFrame::with_fps(fps)?; count]) })
    }

    #[pyfunction]
    fn load(filename: &str) -> PyResult<TasFile> {
        Ok(TasFile { inner: RsTasFile::load(filename)? })
    }

    #[pyfunction]
    fn save(file: &TasFile, filename: &str) -> PyResult<()> {
        Ok(file.inner.save(filename)?)
    }

    // #[pyclass]
    // pub struct PyTasFile {
    //     inner: InternalTasFile,
    // }

    // // Constructor for PyTasFile
    // impl PyTasFile {
    //     fn new_from_inner(inner: InternalTasFile) -> Self {
    //         PyTasFile { inner }
    //     }
    // }

    // #[pymethods]
    // impl PyTasFile {
    //     #[new]
    //     fn new(frames: Option<Vec<PyTasFrame>>) -> PyTasFile {
    //         let frames = frames.unwrap_or_else(|| vec![]);
    //         let inner_frames = frames.into_iter().map(|f| f.inner).collect();
    //         PyTasFile::new_from_inner(InternalTasFile::new(inner_frames))
    //     }

    //     #[staticmethod]
    //     fn from_brandnew(count: usize, frame: PyTasFrame) -> PyTasFile {
    //         PyTasFile::new_from_inner(InternalTasFile::from_brandnew(count, frame.inner))
    //     }

    //     #[staticmethod]
    //     fn from_brandnew_with_fps(count: usize, fps: f32) -> PyResult<PyTasFile> {
    //         match InternalTasFile::from_brandnew_with_fps(count, fps) {
    //             Ok(file) => Ok(PyTasFile::new_from_inner(file)),
    //             Err(e) => Err(PyErr::new::<pyo3::exceptions::PyValueError, _>(
    //                 e.to_string(),
    //             )),
    //         }
    //     }

    //     #[staticmethod]
    //     fn load(filename: &str) -> PyResult<PyTasFile> {
    //         match InternalTasFile::load(filename) {
    //             Ok(file) => Ok(PyTasFile::new_from_inner(file)),
    //             Err(e) => Err(PyErr::new::<pyo3::exceptions::PyIOError, _>(e.to_string())),
    //         }
    //     }

    //     fn save(&self, filename: &str) -> PyResult<()> {
    //         match self.inner.save(filename) {
    //             Ok(()) => Ok(()),
    //             Err(e) => Err(PyErr::new::<pyo3::exceptions::PyIOError, _>(e.to_string())),
    //         }
    //     }

    //     fn clear(&mut self) {
    //         self.inner.clear();
    //     }

    //     #[getter]
    //     fn get_count(&self) -> usize {
    //         self.inner.get_count()
    //     }

    //     fn is_empty(&self) -> bool {
    //         self.inner.is_empty()
    //     }

    //     fn visit(&self, index: usize) -> PyResult<PyTasFrame> {
    //         match self.inner.visit(index) {
    //             Ok(frame) => Ok(PyTasFrame::new_from_inner(*frame)),
    //             Err(e) => Err(PyErr::new::<pyo3::exceptions::PyIndexError, _>(
    //                 e.to_string(),
    //             )),
    //         }
    //     }

    //     fn visit_mut(&mut self, index: usize) -> PyResult<PyTasFrame> {
    //         match self.inner.visit_mut(index) {
    //             Ok(frame) => Ok(PyTasFrame::new_from_inner((*frame).clone())),
    //             Err(e) => Err(PyErr::new::<pyo3::exceptions::PyIndexError, _>(
    //                 e.to_string(),
    //             )),
    //         }
    //     }

    //     fn batchly_visit(&self, index_from: usize, index_to: usize) -> PyResult<Vec<PyTasFrame>> {
    //         match self.inner.batchly_visit(index_from, index_to) {
    //             Ok(frames) => {
    //                 let py_frames: Vec<PyTasFrame> = frames
    //                     .iter()
    //                     .map(|f| PyTasFrame::new_from_inner(*f))
    //                     .collect();
    //                 Ok(py_frames)
    //             }
    //             Err(e) => Err(PyErr::new::<pyo3::exceptions::PyIndexError, _>(
    //                 e.to_string(),
    //             )),
    //         }
    //     }

    //     fn batchly_visit_mut(
    //         &mut self,
    //         index_from: usize,
    //         index_to: usize,
    //     ) -> PyResult<Vec<PyTasFrame>> {
    //         match self.inner.batchly_visit_mut(index_from, index_to) {
    //             Ok(frames) => {
    //                 let py_frames: Vec<PyTasFrame> = frames
    //                     .iter()
    //                     .map(|f| PyTasFrame::new_from_inner((*f).clone()))
    //                     .collect();
    //                 Ok(py_frames)
    //             }
    //             Err(e) => Err(PyErr::new::<pyo3::exceptions::PyIndexError, _>(
    //                 e.to_string(),
    //             )),
    //         }
    //     }

    //     fn append(&mut self, frames: Vec<PyTasFrame>) {
    //         let inner_frames: Vec<TasFrame> = frames.into_iter().map(|f| f.inner).collect();
    //         self.inner.append(&inner_frames);
    //     }

    //     fn insert(&mut self, index: usize, frames: Vec<PyTasFrame>) -> PyResult<()> {
    //         let inner_frames: Vec<TasFrame> = frames.into_iter().map(|f| f.inner).collect();
    //         match self.inner.insert(index, &inner_frames) {
    //             Ok(()) => Ok(()),
    //             Err(e) => Err(PyErr::new::<pyo3::exceptions::PyIndexError, _>(
    //                 e.to_string(),
    //             )),
    //         }
    //     }

    //     fn remove(&mut self, index_from: usize, index_to: usize) -> PyResult<()> {
    //         match self.inner.remove(index_from, index_to) {
    //             Ok(()) => Ok(()),
    //             Err(e) => Err(PyErr::new::<pyo3::exceptions::PyIndexError, _>(
    //                 e.to_string(),
    //             )),
    //         }
    //     }
    // }
}

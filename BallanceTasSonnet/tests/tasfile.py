import sys
import tempfile
from pathlib import Path
import pytest

sys.path.append(str(Path(__file__).resolve().parent.parent / 'python' / 'blctas'))
from blctas import tasfile
from tasfile import TasFile, TasKey


@pytest.fixture(scope='module')
def get_binary_tas_file_path() -> Path:
    return Path(__file__).resolve().parent / 'assets' / 'SR_01.tas'


def test_create_tasfile():
    """Test creating a new TAS file with specified number of frames."""
    # Create a TAS file with 5 frames, each with delta time 0.1
    tas_file: TasFile = tasfile.create(5, 0.1)
    
    assert tas_file.get_count() == 5
    assert not tas_file.is_empty()
    
    # Check that all frames initially have the same delta time
    for i in range(5):
        assert tas_file.get_delta_time(i) == 0.1


def test_load_tasfile():
    """Test loading a TAS file from disk."""
    # Get the path to the sample TAS file
    test_file_path = get_binary_tas_file_path()
    
    # Load the TAS file
    tas_file = tasfile.load(str(test_file_path))
    
    # Basic checks
    assert tas_file.get_count() == 19600  # Should have this frames count
    assert not tas_file.is_empty()


def test_save_tasfile():
    """Test saving a TAS file to disk."""
    # Create a temporary directory.
    with tempfile.TemporaryDirectory() as temp_dir:
        # Fetch temporary file path
        temp_dir_path = Path(temp_dir).resolve()
        temp_file_path = temp_dir_path / 'test.tas'

        # Create a TAS file and save it
        tas_file = tasfile.create(3, 0.05)
        tasfile.save(tas_file, str(temp_file_path))
    
        # Verify the file was created
        assert temp_file_path.is_file()
        
        # Load it back and verify content
        loaded_file = tasfile.load(str(temp_file_path))
        assert loaded_file.get_count() == 3
        for i in range(3):
            assert loaded_file.get_delta_time(i) == 0.05
        


def test_get_set_delta_time():
    """Test getting and setting delta time for frames."""
    tas_file = tasfile.create(3, 0.1)
    
    # Change delta time for the middle frame
    tas_file.set_delta_time(1, 0.25)
    
    # Verify changes
    assert tas_file.get_delta_time(0) == 0.1
    assert tas_file.get_delta_time(1) == 0.25
    assert tas_file.get_delta_time(2) == 0.1
    
    # Test bounds checking
    with pytest.raises(RuntimeError):
        tas_file.get_delta_time(10)  # Index out of range
        
    with pytest.raises(RuntimeError):
        tas_file.set_delta_time(10, 0.5)  # Index out of range


def test_key_press_operations():
    """Test key press operations on frames."""
    tas_file = tasfile.create(2, 0.1)
    
    # Initially all keys should be unpressed
    assert not tas_file.is_key_pressed(0, TasKey.KEY_SPACE)
    assert not tas_file.is_key_pressed(0, TasKey.KEY_UP)
    assert not tas_file.is_key_pressed(1, TasKey.KEY_SHIFT)
    
    # Press SPACE on first frame
    tas_file.set_key_pressed(0, TasKey.KEY_SPACE, True)
    assert tas_file.is_key_pressed(0, TasKey.KEY_SPACE)
    assert not tas_file.is_key_pressed(0, TasKey.KEY_UP)
    
    # Flip the SPACE key (should become unpressed)
    tas_file.flip_key_pressed(0, TasKey.KEY_SPACE)
    assert not tas_file.is_key_pressed(0, TasKey.KEY_SPACE)
    
    # Press SPACE again
    tas_file.flip_key_pressed(0, TasKey.KEY_SPACE)
    assert tas_file.is_key_pressed(0, TasKey.KEY_SPACE)
    
    # Clear all key presses
    tas_file.clear_key_pressed(0)
    assert not tas_file.is_key_pressed(0, TasKey.KEY_SPACE)
    

def test_batch_operations():
    """Test batch operations on multiple frames."""
    tas_file = tasfile.create(5, 0.1)
    
    # Set delta time for frames 1-3 using batch operation
    tas_file.batchly_set_delta_time(1, 3, 0.5)
    
    # Verify individual frame values
    assert tas_file.get_delta_time(0) == 0.1      # Unchanged
    assert tas_file.get_delta_time(1) == 0.5      # Changed
    assert tas_file.get_delta_time(2) == 0.5      # Changed
    assert tas_file.get_delta_time(3) == 0.5      # Changed
    assert tas_file.get_delta_time(4) == 0.1      # Unchanged
    
    # Test batch key operations
    # Initially all keys should be unpressed
    assert not tas_file.is_key_pressed(2, TasKey.KEY_UP)
    assert not tas_file.is_key_pressed(3, TasKey.KEY_UP)
    
    # Press KEY_UP for frames 2-3
    tas_file.batchly_set_key_pressed(2, 3, TasKey.KEY_UP, True)
    assert tas_file.is_key_pressed(2, TasKey.KEY_UP)
    assert tas_file.is_key_pressed(3, TasKey.KEY_UP)
    assert not tas_file.is_key_pressed(1, TasKey.KEY_UP)  # Outside range
    
    # Flip keys for frames 2-3
    tas_file.batchly_flip_key_pressed(2, 3, TasKey.KEY_UP)
    assert not tas_file.is_key_pressed(2, TasKey.KEY_UP)
    assert not tas_file.is_key_pressed(3, TasKey.KEY_UP)
    
    # Test batch clear key presses
    tas_file.set_key_pressed(2, TasKey.KEY_SPACE, True)
    tas_file.set_key_pressed(2, TasKey.KEY_Q, True)
    assert tas_file.is_key_pressed(2, TasKey.KEY_SPACE)
    assert tas_file.is_key_pressed(2, TasKey.KEY_Q)
    
    tas_file.batchly_clear_key_pressed(2, 2)  # Just frame 2
    assert not tas_file.is_key_pressed(2, TasKey.KEY_SPACE)
    assert not tas_file.is_key_pressed(2, TasKey.KEY_Q)


def test_frame_modification_operations():
    """Test adding, inserting, and removing frames."""
    tas_file = tasfile.create(3, 0.1)
    
    # Append 2 more frames
    tas_file.append(2, 0.2)
    assert tas_file.get_count() == 5
    
    # Check that appended frames have correct delta time
    assert tas_file.get_delta_time(3) == 0.2
    assert tas_file.get_delta_time(4) == 0.2
    
    # Insert 1 frame at position 1
    tas_file.insert(1, 1, 0.15)
    assert tas_file.get_count() == 6
    assert tas_file.get_delta_time(1) == 0.15  # New frame
    assert tas_file.get_delta_time(2) == 0.1   # Old frame 1 (now shifted)
    
    # Remove frames 2-3 (inclusive)
    tas_file.remove(2, 3)
    assert tas_file.get_count() == 4
    
    # Check that the right frames were removed
    assert tas_file.get_delta_time(0) == 0.1   # Original first frame
    assert tas_file.get_delta_time(1) == 0.15  # Inserted frame
    assert tas_file.get_delta_time(2) == 0.1   # Original third frame (shifted)
    assert tas_file.get_delta_time(3) == 0.2   # Original fourth frame (shifted)


def test_clear_operation():
    """Test clearing all frames from the TAS file."""
    tas_file = tasfile.create(3, 0.1)
    assert tas_file.get_count() == 3
    assert not tas_file.is_empty()
    
    tas_file.clear()
    assert tas_file.get_count() == 0
    assert tas_file.is_empty()


def test_edge_cases():
    """Test edge cases and error conditions."""
    tas_file = tasfile.create(1, 0.1)
    
    # Test batch operations with invalid ranges
    with pytest.raises(RuntimeError):
        tas_file.batchly_set_delta_time(0, 5, 0.2)  # End index out of range
        
    with pytest.raises(RuntimeError):
        tas_file.batchly_set_delta_time(2, 0, 0.2)  # Invalid range (end < start)
        
    with pytest.raises(RuntimeError):
        tas_file.batchly_set_key_pressed(0, 5, TasKey.KEY_SPACE, True)  # Out of range
        
    with pytest.raises(RuntimeError):
        tas_file.batchly_flip_key_pressed(0, 5, TasKey.KEY_SPACE)  # Out of range
        
    with pytest.raises(RuntimeError):
        tas_file.batchly_clear_key_pressed(0, 5)  # Out of range
        
    with pytest.raises(RuntimeError):
        tas_file.remove(0, 5)  # End index out of range
        
    with pytest.raises(RuntimeError):
        tas_file.remove(2, 0)  # Invalid range (end < start)


def test_available_keys():
    """Test that all expected keys are available."""
    tas_file = tasfile.create(1, 0.1)
    
    all_keys = [
        TasKey.KEY_UP, TasKey.KEY_DOWN, TasKey.KEY_LEFT, TasKey.KEY_RIGHT,
        TasKey.KEY_SHIFT, TasKey.KEY_SPACE, TasKey.KEY_Q, TasKey.KEY_ESC, TasKey.KEY_ENTER
    ]
    
    # Test that we can interact with all keys without errors
    for key in all_keys:
        # Initially should be unpressed
        assert not tas_file.is_key_pressed(0, key)
        
        # Press the key
        tas_file.set_key_pressed(0, key, True)
        assert tas_file.is_key_pressed(0, key)
        
        # Flip the key (release it)
        tas_file.flip_key_pressed(0, key)
        assert not tas_file.is_key_pressed(0, key)
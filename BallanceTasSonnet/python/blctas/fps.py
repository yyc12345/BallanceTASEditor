import math


def to_fps(delta_time: float) -> float:
    """
    Convert delta time to FPS value.

    :param delta_time: The delta time.
    :return: The FPS in float value.
    """
    if delta_time <= 0.0:
        raise RuntimeError("delta time should not be zero or negative")
    else:
        return 1.0 / delta_time


def to_int_fps(delta_time: float) -> int:
    """
    Convert delta time to integer FPS value.

    :param delta_time: The delta time.
    :return: The integer FPS (rounded down).
    """
    return int(math.floor(to_fps(delta_time)))


def from_fps(fps: float) -> float:
    """
    Convert FPS value to delta time.

    :param fps: The FPS value.
    :return: The delta time.
    """
    if fps <= 0.0:
        raise RuntimeError("fps should not be zero or negative")
    else:
        return 1.0 / fps


def from_int_fps(fps: int) -> float:
    """
    Convert integer FPS value to delta time.

    :param fps: The integer FPS value.
    :return: The delta time.
    """
    return from_fps(float(fps))

import pathlib

def fetch_svg_files() -> tuple[pathlib.Path, ...]:
    return tuple(file for file in pathlib.Path("./").glob("*.svg"))

def generate_windows_script(svg_files: tuple[pathlib.Path, ...]) -> None:
    with open("./BuildIcons.bat", "w", encoding="utf-8") as f:
        for svg_file in svg_files:
            png_file = f'{svg_file.stem}.png'
            ico_file = f'{svg_file.stem}.ico'
            f.write(f"inkscape --without-gui {svg_file} -o {png_file} -w 250 -h 256\n")
            f.write(f"magick {png_file} -define icon:auto-resize=256,128,64,48,32,16 {ico_file}\n")

def generate_linux_script(svg_files: tuple[pathlib.Path, ...]) -> None:
    with open("./BuildIcons.sh", "w", encoding="utf-8") as f:
        for svg_file in svg_files:
            png_file = f'{svg_file.stem}.png'
            ico_file = f'{svg_file.stem}.ico'
            f.write(f"inkscape --without-gui {svg_file} -o {png_file} -w 250 -h 256\n")
            f.write(f"magick {png_file} -define icon:auto-resize=256,128,64,48,32,16 {ico_file}\n")

def main():
    print("Fetching SVG files...")
    svg_files = fetch_svg_files()
    print("Building Windows script...")
    generate_windows_script(svg_files)
    print("Building Linux script...")
    generate_linux_script(svg_files)
    print("Done!")

if __name__ == "__main__":
    main()

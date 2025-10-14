#!/usr/bin/env python3
import os, subprocess, sys

def main():
    out_dir = os.path.join('Documentation', 'media', 'demo')
    os.makedirs(out_dir, exist_ok=True)
    # Expect frames pre-rendered by in-engine capture (not implemented). Assemble to GIF using ffmpeg
    # Requires ffmpeg in PATH
    input_pattern = os.path.join(out_dir, 'frame_%05d.png')
    output_gif = os.path.join(out_dir, 'demo.gif')
    cmd = ['ffmpeg', '-y', '-framerate', '30', '-i', input_pattern, '-vf', 'palettegen=stats_mode=diff [p]; [0:v][p] paletteuse', output_gif]
    print('Running:', ' '.join(cmd))
    subprocess.run(cmd, check=False)

if __name__ == '__main__':
    sys.exit(main())



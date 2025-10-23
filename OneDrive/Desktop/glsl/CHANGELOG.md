# Changelog

All notable changes to Chroma will be documented in this file.

## [0.1.0] - 2025-01-22

### 🎉 Initial Release

**Chroma v0.1.0** marks the first stable release of our GPU-accelerated terminal visualizer, bringing beautiful ASCII art shader effects to your terminal with unprecedented performance.

### ✨ Core Features Added

#### Visual Patterns (18+)
- **Plasma** - Classic plasma effect with flowing colors
- **Waves** - Sine wave patterns with interference
- **Ripples** - Concentric ripple effects  
- **Vortex** - Spiral vortex patterns
- **Noise** - Perlin noise-based textures
- **Geometric** - Mathematical geometric patterns
- **Voronoi** - Voronoi cell patterns
- **Truchet** - Truchet tile patterns
- **Hexagonal** - Hexagonal grid patterns
- **Interference** - Wave interference patterns
- **Fractal** - Mandelbrot-style fractals
- **Glitch** - Digital glitch effects
- **Spiral** - Spiral and logarithmic patterns
- **Rings** - Concentric ring patterns
- **Grid** - Grid-based patterns
- **Diamonds** - Diamond-shaped patterns
- **Sphere** - 3D sphere projections
- **Octgrams** - Eight-pointed star patterns
- **Warped FBM** - Fractional Brownian Motion with warping

#### Color Modes (10+)
- **Rainbow** - Full spectrum rainbow colors
- **Monochrome** - Grayscale variations
- **Duotone** - Two-color schemes
- **Warm** - Warm color palette
- **Cool** - Cool color palette
- **Neon** - Bright neon colors
- **Pastel** - Soft pastel colors
- **Cyberpunk** - Cyberpunk-inspired colors
- **Warped** - Distorted color mapping
- **Chromatic** - Chromatic aberration effects

#### ASCII Palettes (15+)
- **Standard** - Basic ASCII characters
- **Blocks** - Block characters (█, ▓, ▒, ░)
- **Circles** - Circle characters (●, ○, ◐, ◑)
- **Smooth** - Smooth gradient characters
- **Braille** - Braille pattern characters
- **Geometric** - Geometric shapes
- **Mixed** - Mixed character types
- **Dots** - Dot-based characters
- **Shades** - Shading characters
- **Lines** - Line-based characters
- **Triangles** - Triangle characters
- **Arrows** - Arrow characters
- **Powerline** - Powerline symbols
- **Boxdraw** - Box drawing characters
- **Extended** - Extended Unicode characters
- **Simple** - Minimal character set

### 🚀 Technical Features

#### GPU Acceleration
- **WebGPU Compute Shaders** - All rendering powered by GPU for maximum performance
- **Cross-platform Support** - Windows, Linux, macOS compatibility
- **High Performance** - Achieves 2000+ FPS on modern hardware
- **Efficient Memory Management** - Minimal CPU overhead with optimized buffer handling

#### Audio Visualization (Optional Feature)
- **Real-time FFT Analysis** - Extracts bass, mid, treble frequencies
- **Beat Detection** - Reactive effects triggered by audio beats
- **Audio Device Selection** - Choose your input device
- **Reactive Parameters** - Frequency, amplitude, and effects respond to audio
- **Configurable Influence** - Adjust how much each frequency band affects visuals

#### Configuration System
- **TOML Configuration Files** - Human-readable configuration format
- **Live Reloading** - Edit config files while running for instant changes
- **CLI Parameter Overrides** - Command-line arguments take precedence
- **Configuration Presets** - 20+ example configurations included
- **Save/Load Configurations** - Automatic deduping via hashing

#### Interactive Controls
- **Real-time Parameter Adjustment** - Live keyboard controls
- **Pattern Cycling** - Cycle through all 18+ visual patterns
- **Color Mode Cycling** - Switch between 10+ color schemes
- **ASCII Palette Cycling** - Choose from 15+ character sets
- **Parameter Controls** - Adjust brightness, contrast, frequency, amplitude, speed
- **Randomization** - Discover new effects with random parameter generation

### 🎮 User Experience

#### Installation & Setup
- **Simple Installation** - Single `cargo build --release` command
- **Arch Linux Package** - Available via AUR (`chroma-visualizer-git`)
- **Cross-platform Binary** - Works on Windows, Linux, macOS
- **Minimal Dependencies** - Only requires Rust and GPU drivers

#### Usage
- **Command-line Interface** - Comprehensive CLI with help system
- **Custom Shaders** - Load your own WGSL shader files
- **Example Configurations** - 20+ preset configurations to get started
- **Audio Integration** - Optional audio visualization with system audio

### 📚 Documentation & Support

#### Comprehensive Documentation
- **README.md** - Complete usage guide with examples
- **Architecture Documentation** - Detailed technical architecture
- **Control Reference** - Complete keyboard controls documentation
- **Palette Guide** - All ASCII palettes explained
- **Configuration Guide** - TOML configuration examples
- **Audio Setup Guide** - Audio visualization setup instructions

#### Developer Resources
- **Contributing Guidelines** - Clear contribution process
- **Test Suite** - Comprehensive test coverage
- **Code Examples** - Custom shader templates
- **Performance Benchmarks** - Performance optimization guidelines

### 🔧 Development & Maintenance

#### Code Quality
- **Rust Best Practices** - Modern Rust with proper error handling
- **Modular Architecture** - Clean separation of concerns
- **Comprehensive Testing** - Unit tests, integration tests, and benchmarks
- **Performance Optimization** - GPU-accelerated rendering pipeline

#### Community & Support
- **Open Source** - MIT licensed for maximum accessibility
- **Active Development** - Regular updates and improvements
- **Community Contributions** - Welcome contributions from developers
- **Issue Tracking** - GitHub issues for bug reports and feature requests

---

**Chroma v0.1.0** represents a significant milestone in terminal visualization, bringing GPU-accelerated shader effects to ASCII art with unprecedented performance and visual quality. This release establishes Chroma as the premier tool for terminal-based visual effects and audio visualization.

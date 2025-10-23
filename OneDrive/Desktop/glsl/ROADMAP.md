# Chroma Development Roadmap

## Overview

This roadmap outlines planned improvements and features for Chroma v0.2.0 and beyond. Features are categorized by type and priority to guide development phases.

## Development Phases

### Phase 1: Performance & Stability (v0.1.1)
**Priority: High** - Foundation improvements for better user experience

#### Optimization/Performance
- **#1 Shader Caching** - Pre-compile common shaders to reduce startup latency
- **#2 Dynamic Resolution Scaling** - Auto-adjust terminal cell size for better performance on high-res monitors  
- **#3 WGPU Buffer Pipelining** - Optimize data transfer between CPU and GPU
- **#12 Frame Limiter** - Option to cap FPS to save CPU/GPU resources (e.g., `--fps 30`)

#### Quality of Life
- **#7 Terminal Resize Handling** - More graceful and instant re-calculation of render area on terminal resize events
- **#25 Exit Confirmation** - Optional prompt when quitting (Q/Esc) to prevent accidental closure

### Phase 2: User Experience (v0.1.2)
**Priority: High** - Core usability improvements

#### Quality of Life
- **#4 Startup Config Picker** - An interactive TUI to select a saved config file at launch
- **#5 Smooth Palette Cycling** - Implement a gradual fade/cross-dissolve between palette cycles
- **#6 On-Screen Control Hints** - A small, toggleable HUD showing the current key controls

#### Features
- **#8 Custom Keybinds** - Allow users to remap controls in the config file
- **#11 Pause/Play Toggle** - A keybind (e.g., Spacebar) to freeze/unfreeze the animation

### Phase 3: Enhanced Features (v0.2.0)
**Priority: Medium** - Feature expansion and customization

#### Features
- **#9 More Shader Uniforms** - Expose additional controllable parameters (e.g., color intensity, rotation axis) to shaders
- **#10 Color Palette Editor (TUI)** - An in-app editor to define custom color schemes and save them

#### Audio Enhancements
- **#13 Peak Hold/Decay** - Adjust how quickly audio-reactive parameters return to baseline
- **#14 Audio Sensitivity/Gain Control** - CLI/config option to globally adjust audio input level
- **#15 Microphone Exclusion Zones** - Option to filter out very low/high frequencies from audio analysis

### Phase 4: Visual Expansion (v0.2.1)
**Priority: Medium** - New visual patterns and effects

#### Visual Patterns
- **#16 Fluid Dynamics** - Simple simulation of flowing liquid or smoke
- **#17 Metaballs** - 2D or 3D implicit surface rendering

#### Color & Palette Enhancements
- **#18 Heatmap Color Mode** - Colors based on the calculated 'energy' or 'density' of the shader output
- **#19 Emoji ASCII Palette** - Use a curated set of Unicode emojis for rendering

### Phase 5: Distribution & Documentation (v0.2.2)
**Priority: Low** - Packaging and documentation improvements

#### Packaging
- **#20 Static Musl Builds** - Provide portable binaries for various Linux systems
- **#21 Homebrew/MacPorts** - Official packaging for macOS users
- **#22 Windows Installer/Scoop** - Easier installation for Windows users

#### Documentation
- **#23 Advanced Shader Guide** - Walkthrough for writing complex custom WGSL shaders
- **#24 Performance Tuning Guide** - Tips for users with low-end hardware

## Implementation Priority Matrix

| Feature | Category | Impact | Effort | Priority |
|---------|----------|--------|--------|----------|
| Shader Caching | Optimization | High | Medium | P0 |
| Terminal Resize Handling | QoL | High | Low | P0 |
| Frame Limiter | Optimization | Medium | Low | P0 |
| Startup Config Picker | QoL | High | Medium | P1 |
| Custom Keybinds | Feature | High | Medium | P1 |
| Smooth Palette Cycling | QoL | Medium | Medium | P1 |
| On-Screen Control Hints | QoL | Medium | Low | P1 |
| Pause/Play Toggle | Feature | Medium | Low | P1 |
| More Shader Uniforms | Feature | High | High | P2 |
| Audio Sensitivity Control | Audio | Medium | Low | P2 |
| Color Palette Editor | Feature | Medium | High | P2 |
| Fluid Dynamics Pattern | Visual | Medium | High | P3 |
| Metaballs Pattern | Visual | Medium | High | P3 |
| Heatmap Color Mode | Visual | Low | Medium | P3 |
| Emoji ASCII Palette | Visual | Low | Low | P3 |
| Static Builds | Packaging | Low | Medium | P4 |
| Advanced Shader Guide | Documentation | Low | High | P4 |

## Technical Considerations

### Performance Optimizations
- **Shader Caching**: Implement a cache system for compiled WGSL shaders
- **Buffer Pipelining**: Use multiple GPU buffers for async data transfer
- **Resolution Scaling**: Detect terminal capabilities and adjust rendering accordingly

### Architecture Changes
- **Config System**: Extend TOML configuration to support keybind remapping
- **TUI Components**: Add interactive elements for config selection and palette editing
- **Audio Pipeline**: Enhance audio processing with configurable sensitivity and filtering

### Compatibility
- **Cross-platform**: Ensure all features work on Windows, Linux, and macOS
- **Terminal Support**: Maintain compatibility with various terminal emulators
- **GPU Drivers**: Support both Vulkan and DirectX backends

## Development Guidelines

### Code Quality
- Maintain existing Rust best practices
- Add comprehensive tests for new features
- Follow the established modular architecture
- Document all public APIs

### User Experience
- Maintain backward compatibility with existing configs
- Provide clear migration paths for breaking changes
- Include helpful error messages and documentation
- Test on various hardware configurations

### Performance
- Benchmark all performance-critical changes
- Maintain 2000+ FPS target on modern hardware
- Optimize for both high-end and low-end systems
- Provide performance tuning options

## Contributing

Contributions are welcome for any roadmap items! Please:

1. **Create an issue** for the feature you want to work on
2. **Fork the repository** and create a feature branch
3. **Follow the existing code style** and architecture patterns
4. **Add tests** for new functionality
5. **Update documentation** as needed
6. **Submit a pull request** with a clear description

## Release Schedule

- **v0.1.1** (Performance & Stability) - Target: 2-3 weeks
- **v0.1.2** (User Experience) - Target: 4-5 weeks  
- **v0.2.0** (Enhanced Features) - Target: 8-10 weeks
- **v0.2.1** (Visual Expansion) - Target: 12-14 weeks
- **v0.2.2** (Distribution & Documentation) - Target: 16-18 weeks

*Timelines are estimates and may vary based on development capacity and community contributions.*

---

**Note**: This roadmap is a living document and will be updated as features are implemented and new requirements emerge. Community feedback and contributions are essential for prioritizing features effectively.

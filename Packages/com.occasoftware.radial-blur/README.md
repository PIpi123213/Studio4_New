# README

## Contents

- About
- Install
- Use
- Public API
- Troubleshooting
- Requirements
- Support
- Feedback


## About

Radial Blur is a Renderer Feature and API. It enables you to use Radial (Zoom) Blur in your Unity URP project. You can customize the Intensity of the effect, the Sample Count, and the Center (Origin) point.

## Install

1. git clone to your Packages folder.


## Use

### Add the Radial Blur Feature to your Renderer

1. Import the Radial Blur asset to your project.
2. Open your Forward Renderer Data asset.
3. Click **Add Renderer Feature**.
4. Select **Radial Blur Feature**.

### Make a Radial Blur Volume Override

1. Select **GameObject > Volume > Global Volume**.
2. In the Volume Component in the Inspector pane, click **New**.
3. Click **Add Override**.
4. Select **OccaSoftware > Radial Blur**

## Public API

The RadialBlurManager class includes the following public methods. These can be viewed directly in source in the .cs file.

### SetIntensity

```cs
public void SetIntensity(float intensity);
```

Sets the intensity of the Radial Blur filter

### GetIntensity

```cs
public float GetIntensity();
```

Gets the intensity of the Radial Blur filter

# SetCenter 

```cs
public void SetCenter(Vector2 center);
```

Sets the Center (Origin) of the Radial Blur filter. [0,0] is the Screen Center.

### SetCenterFromScreenPoint

```cs
public void SetCenterFromScreenPoint(Vector2 screenPoint);
```

Sets the Center (Origin) of the Radial Blur filter from screen point coordinates, measured [0,0] at bottom left to [Screen.width, Screen.height] at top right.

### GetCenter

```cs
public Vector2 GetCenter();
```

Gets the current Center (Origin) of the Radial Blur filter. [0,0] is the Screen Center.

### SetSampleCount

```cs
public void SetSampleCount(int sampleCount);
```

Sets the number of target number of samples to be used for the Radial Blur filter.

### GetSampleCount

```cs
public int GetSampleCount();
```

Gets the current number of samples being used for the Radial Blur filter.

### GetDelay

```cs
public float GetDelay();
```

Gets the current delay being used.

### SetDelay

```cs
public void SetDelay(float delay);
```

Sets the start offset to be used for the Radial Blur filter.


## Troubleshoot

1. Verify that the Radial Blur Renderer Feature is included in your Forward Renderer Data asset.
2. Verify that you have a Radial Blur override present in your scene.
3. Verify that you have a RadialBlurShader present in your project.


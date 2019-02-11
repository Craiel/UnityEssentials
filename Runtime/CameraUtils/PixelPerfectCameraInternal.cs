namespace Craiel.UnityEssentials.Runtime.CameraUtils
{
  using System;
  using UnityEngine;

  public class PixelPerfectCameraInternal
  {
    private readonly PixelPerfectCamera cameraComponent;

    // -------------------------------------------------------------------
    // Constructor
    // -------------------------------------------------------------------
    public PixelPerfectCameraInternal(PixelPerfectCamera component)
    {
      this.cameraComponent = component;
    }

    // -------------------------------------------------------------------
    // Public
    // -------------------------------------------------------------------
    public bool CropFrameXAndY;
    public bool CropFrameXOrY;
    public bool UseStretchFill;
    public int Zoom = 1;
    public bool UseOffscreenRT;
    public int OffscreenRTWidth;
    public int OffscreenRTHeight;
    public Rect PixelRect = Rect.zero;
    public float OrthoSize = 1f;
    public float UnitsPerPixel;
    public float OriginalOrthoSize;
    public bool HasPostProcessLayer;

    public void CalculateCameraProperties(int screenWidth, int screenHeight)
    {
      int assetsPpu = this.cameraComponent.AssetsPPU;
      int refResolutionX = this.cameraComponent.RefResolutionX;
      int refResolutionY = this.cameraComponent.RefResolutionY;
      bool upscaleRt = this.cameraComponent.UpscaleRT;
      bool pixelSnapping = this.cameraComponent.PixelSnapping;
      bool cropFrameX = this.cameraComponent.CropFrameX;
      bool cropFrameY = this.cameraComponent.CropFrameY;
      bool stretchFill = this.cameraComponent.StretchFill;
      this.CropFrameXAndY = cropFrameY && cropFrameX;
      this.CropFrameXOrY = cropFrameY || cropFrameX;
      this.UseStretchFill = this.CropFrameXAndY && stretchFill;
      this.Zoom = Math.Max(1, Math.Min(screenHeight / refResolutionY, screenWidth / refResolutionX));
      this.UseOffscreenRT = false;
      this.OffscreenRTWidth = 0;
      this.OffscreenRTHeight = 0;
      if (this.CropFrameXOrY)
      {
        if (!upscaleRt)
        {
          if (this.UseStretchFill)
          {
            this.UseOffscreenRT = true;
            this.OffscreenRTWidth = this.Zoom * refResolutionX;
            this.OffscreenRTHeight = this.Zoom * refResolutionY;
          }
        }
        else
        {
          this.UseOffscreenRT = true;
          if (this.CropFrameXAndY)
          {
            this.OffscreenRTWidth = refResolutionX;
            this.OffscreenRTHeight = refResolutionY;
          }
          else if (cropFrameY)
          {
            this.OffscreenRTWidth = screenWidth / this.Zoom / 2 * 2;
            this.OffscreenRTHeight = refResolutionY;
          }
          else
          {
            this.OffscreenRTWidth = refResolutionX;
            this.OffscreenRTHeight = screenHeight / this.Zoom / 2 * 2;
          }
        }
      }
      else if (upscaleRt && this.Zoom > 1)
      {
        this.UseOffscreenRT = true;
        this.OffscreenRTWidth = screenWidth / this.Zoom / 2 * 2;
        this.OffscreenRTHeight = screenHeight / this.Zoom / 2 * 2;
      }

      this.PixelRect = Rect.zero;
      if (this.CropFrameXOrY && !upscaleRt && !this.UseStretchFill)
      {
        if (this.CropFrameXAndY)
        {
          this.PixelRect.width = this.Zoom * refResolutionX;
          this.PixelRect.height = this.Zoom * refResolutionY;
        }
        else if (cropFrameY)
        {
          this.PixelRect.width = screenWidth;
          this.PixelRect.height = (float) (this.Zoom * refResolutionY);
        }
        else
        {
          this.PixelRect.width = (float) (this.Zoom * refResolutionX);
          this.PixelRect.height = (float) screenHeight;
        }

        this.PixelRect.x = (float) ((screenWidth - (int) this.PixelRect.width) / 2);
        this.PixelRect.y = (float) ((screenHeight - (int) this.PixelRect.height) / 2);
      }
      else if (this.UseOffscreenRT)
        this.PixelRect = new Rect(0.0f, 0.0f, (float) this.OffscreenRTWidth, (float) this.OffscreenRTHeight);

      if (cropFrameY)
        this.OrthoSize = (float) refResolutionY * 0.5f / (float) assetsPpu;
      else if (cropFrameX)
      {
        float num = !(this.PixelRect == Rect.zero)
          ? this.PixelRect.width / this.PixelRect.height
          : (float) screenWidth / (float) screenHeight;
        this.OrthoSize = (float) ((double) refResolutionX / (double) num * 0.5) / (float) assetsPpu;
      }
      else
        this.OrthoSize = !upscaleRt || this.Zoom <= 1
          ? (!(this.PixelRect == Rect.zero) ? this.PixelRect.height : (float) screenHeight) * 0.5f /
            (float) (this.Zoom * assetsPpu)
          : (float) this.OffscreenRTHeight * 0.5f / (float) assetsPpu;

      if (upscaleRt || !upscaleRt && pixelSnapping)
        this.UnitsPerPixel = 1f / (float) assetsPpu;
      else
        this.UnitsPerPixel = 1f / (float) (this.Zoom * assetsPpu);
    }

    public Rect CalculatePostRenderPixelRect(float cameraAspect, int screenWidth, int screenHeight)
    {
      Rect rect = new Rect();
      if (this.UseStretchFill)
      {
        if ((double) ((float) screenWidth / (float) screenHeight) > (double) cameraAspect)
        {
          rect.height = screenHeight;
          rect.width = screenHeight * cameraAspect;
          rect.x = (float) ((screenWidth - (int) rect.width) / 2);
          rect.y = 0.0f;
        }
        else
        {
          rect.width = screenWidth;
          rect.height = screenWidth / cameraAspect;
          rect.y = (float) ((screenHeight - (int) rect.height) / 2);
          rect.x = 0.0f;
        }
      }
      else
      {
        rect.height = this.Zoom * this.OffscreenRTHeight;
        rect.width = this.Zoom * this.OffscreenRTWidth;
        rect.x = (screenWidth - (int) rect.width) / 2;
        rect.y = (screenHeight - (int) rect.height) / 2;
      }

      return rect;
    }
  }
}
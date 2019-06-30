/**
 * Copyright (c) Audectra e.U. - All Rights Reserved
 * This effect is part of Audectra.
 */

using System;
using System.Linq;
using System.Collections.Generic;

using SkiaSharp;
using Audectra.Extensions.Sdk.V1;

namespace Audectra.Extensions.Effects
{
    [MinWidth(8)]
    [LandscapeAspectRatio]
    [EffectExtension("Walking Blobs", "Audectra", "v0.0.1")]
    class WalkingBlobs : EffectExtensionBase
    {
        private IEffectApi _api;
        private IRender _render;

        private float _blobSize;
        private float _blobSpeed;
        private float _blobWalkTime;
        private SKColor _blobColor;
        private IList<WalkingBlob> _blobs;

        private readonly object _lock = new object();

        private enum SettingId
        {
            Color,
            Size,
            Speed
        }

        private enum TriggerId
        {
            BlobAndWalk,
        }

        public WalkingBlobs(IEffectApi api, int width, int height) : base(width, height)
        {
            _api = api;
            _render = _api.CreateRender();

            _blobs = new List<WalkingBlob>();

            _blobSize = width / 16f;
            _blobSpeed = width / 2;
            _blobWalkTime = 0.375f;
            _blobColor = new SKColor(0, 255, 128);
        }

        public override IRender Render(float dt)
        {
            using (var canvas = _render.CreateCanvas())
            {
                canvas.Clear();
                
                lock (_lock)
                {
                    UpdateAndRenderBlobs(dt, canvas);
                }
            }

            return _render;
        }

        private void UpdateAndRenderBlobs(float dt, SKCanvas canvas)
        {
            foreach (var blob in _blobs.ToList())   // Iterate through copy. 
            {
                // Update walking blob.
                blob.Update(dt);

                // If blob has walked away, remove it and continue. 
                if (blob.XCenterPos - blob.Size / 2 > Width)
                {
                    _blobs.Remove(blob);
                    continue;
                }

                // Render blob onto canvas.
                blob.Render(canvas);
            }
        }

        public override void GenerateSettings(ISettingsBuilder settingsBuilder)
        {
            settingsBuilder.PageBegin();
            settingsBuilder.AddColorGroup(_blobColor, (uint)SettingId.Color);

            settingsBuilder.GroupBegin("Size");
            settingsBuilder.AddBindableSlider(_blobSize, 0, Width / 8f, (uint)SettingId.Size);
            settingsBuilder.GroupEnd();

            settingsBuilder.GroupBegin("Speed");
            settingsBuilder.AddSlider(_blobSpeed, Width / 8, Width, (uint)SettingId.Speed);
            settingsBuilder.GroupEnd();

            settingsBuilder.GroupBegin("Walk");
            settingsBuilder.AddBindableTrigger((uint)TriggerId.BlobAndWalk);
            settingsBuilder.GroupEnd();
            
            settingsBuilder.PageEnd();
        }

        public override void OnSettingChanged(uint settingId, SettingValue value)
        {
            switch ((SettingId) settingId)
            {
                case SettingId.Color:
                    _blobColor = value;
                    break;

                case SettingId.Size:
                    _blobSize = value;
                    break;

                case SettingId.Speed:
                    _blobSpeed = value;
                    break;
            }
        }

        public override void OnTrigger(uint triggerId, bool risingEdge)
        {
            if (!risingEdge)
                return;

            switch ((TriggerId) triggerId)
            {
                case TriggerId.BlobAndWalk:
                {
                    lock (_lock)
                    {
                        if (_blobSize > 0)
                        {
                            var walkingBlob = new WalkingBlob
                            {
                                XCenterPos = 0,
                                YCenterPos = Height / 2f,
                                Size = _blobSize,
                                WalkingTime = _blobWalkTime,
                                WalkingSpeed = _blobSpeed,
                                Color = _blobColor
                            };

                            _blobs.Add(walkingBlob);
                        }

                        foreach (var blob in _blobs)
                            blob.StartWalking();
                    }

                    break;
                }
            }
        }
    }

    class WalkingBlob
    {
        private float _walkingTime;

        public float XCenterPos { get; set; }
        public float YCenterPos { get; set; }
        public float Size { get; set; }
        public float WalkingTime { get; set; }
        public float WalkingSpeed { get; set; }
        public SKColor Color { get; set; }
        public bool IsWalking { get; private set; }

        public void Update(float dt)
        {
            if (!IsWalking)
                return;

            _walkingTime += dt;
            float speed = CalculateSpeed();

            XCenterPos += speed * dt;
        }

        public void Render(SKCanvas canvas)
        {
            float x = XCenterPos;
            float y = YCenterPos;

            var paint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                BlendMode = SKBlendMode.Plus,
                Shader = SKShader.CreateRadialGradient(
                    new SKPoint(x, y),
                    Size, new[] {Color, SKColors.Black},
                    null, SKShaderTileMode.Clamp),
            };

            canvas.DrawOval(x, y, Size, Size, paint);
            paint.Dispose();
        }

        private float CalculateSpeed()
        {
            float t = _walkingTime;
            float T = WalkingTime;

            if (t > T)
            {
                IsWalking = false;
                return 0f;
            }
            
            return 40 * WalkingSpeed * (t / T - 0.4f) * (t / T) * (t / T) * (float)Math.Cos(t / T * Math.PI / 2.0);
        }

        public void StartWalking()
        {
            _walkingTime = 0;
            IsWalking = true;
        }
    }
}

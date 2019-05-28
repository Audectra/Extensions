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
    [EffectExtension("Zooming Blobs", "Audectra", "v0.0.1")]
    class ZoomingBlobs : EffectExtensionBase
    {
        private IEffectApi _api;
        private IRender _render;

        private class BlobItem
        {
            public float Position;
            public float Life;
            public float MaxLife;
            public float MaxSize;
            public SKColor Color;
        }

        private float _maxBlobLife;
        private float _maxBlobSize;
        private IList<BlobItem> _blobs;
        private Random _rand;

        private readonly object _blobsLock = new object();

        private enum SettingId
        {
            BlobSize,
            BlobLife,
        }

        private enum TriggerId
        {
            BlobBoom = 0,
        }

        public ZoomingBlobs(IEffectApi api, int width, int height) : base(width, height)
        {
            _api = api;
            _render = _api.CreateRender();

            _maxBlobLife = 0.5f;
            _maxBlobSize = width / 8f;
            _rand = new Random();
            _blobs = new List<BlobItem>();
        }

        public override IRender Render(float dt)
        {
            UpdateBlobs(dt);

            return RenderBlobs();
        }

        private void UpdateBlobs(float dt)
        {
            lock (_blobsLock) {
                foreach (var bar in _blobs.ToArray())
                {
                    bar.Life -= dt;

                    if (bar.Life < 0)
                        _blobs.Remove(bar);
                }
            }
        }

        private IRender RenderBlobs()
        {
            float y = Height / 2f;

            using (var canvas = _render.CreateCanvas())
            {
                canvas.Clear();

                lock (_blobsLock)
                {
                    foreach (var blob in _blobs)
                    {
                        float scale = blob.Life / blob.MaxLife;
                        float size = (1 - scale) * blob.MaxSize;
                        float x = blob.Position;

                        var paint = new SKPaint
                        {
                            IsAntialias = true,
                            Style = SKPaintStyle.Fill,
                            Shader = SKShader.CreateRadialGradient(
                                new SKPoint(x, y),
                                size, 
                                new[] {blob.Color.WithScale(scale), SKColors.Black},
                                null, SKShaderTileMode.Clamp),
                        };

                        canvas.DrawOval(x, y, size, size, paint);
                        paint.Dispose();
                    }
                }
            }

            return _render;
        }

        public override void GenerateSettings(ISettingsBuilder settingsBuilder)
        {
            settingsBuilder.PageBegin();

            settingsBuilder.GroupBegin("Blob Size");
            settingsBuilder.AddBindableSlider(_maxBlobSize, 0, Width / 2f, (uint)SettingId.BlobSize);
            settingsBuilder.GroupEnd();

            settingsBuilder.GroupBegin("Blob Life");
            settingsBuilder.AddBindableSlider(_maxBlobLife, 0.1f, 1.0f, (uint)SettingId.BlobLife);
            settingsBuilder.GroupEnd();

            settingsBuilder.GroupBegin("Trigger");
            settingsBuilder.AddBindableTrigger((uint)TriggerId.BlobBoom);
            settingsBuilder.GroupEnd();
            
            settingsBuilder.PageEnd();
        }

        public override void OnSettingChanged(uint settingId, SettingValue value)
        {
            switch ((SettingId) settingId)
            {
                case SettingId.BlobSize:
                    _maxBlobSize = value;
                    break;

                case SettingId.BlobLife:
                    _maxBlobLife = value;
                    break;
            }
        }

        public override void OnTrigger(uint triggerId, bool risingEdge)
        {
            if (!risingEdge)
                return;

            switch ((TriggerId) triggerId)
            {
                case TriggerId.BlobBoom:
                {
                    var blob = CreateBlob();

                    if (blob.MaxLife > 0 &&  blob.MaxSize > 0)
                        lock (_blobsLock)
                        {
                            _blobs.Add(blob);
                        }
                }
                break;
            }
        }

        private BlobItem CreateBlob()
        {
            return new BlobItem
            {
                Position = (float)_rand.NextDouble() * Width,
                Life = _maxBlobLife,
                MaxLife = _maxBlobLife,
                MaxSize = _maxBlobSize,
                Color = _api.CreateRandomColor(),
            };
        }
    }
}

/**
 * Copyright (c) Audectra e.U. - All Rights Reserved
 * This effect is part of Audectra.
 */

using System;

using Audectra.GUI;
using Audectra.Graphics;
using Audectra.Graphics.Effects;

namespace Audectra.Extensions.Effects
{
    class ChromaBars : EffectBase, IExtension
    {
        private IEffectHelper _helper;
        private RgbColor _color;
        private IRgbRender _render;

        private const int NumBars = 12;
        private int _barSize;
        private float _threshold;

        private enum ValueId
        {
            Color = 0,
            ThresholdValue
        }

        public ChromaBars() { }

        public ChromaBars(IEffectHelper effectHelper, int height, int width) : base(height, width)
        {
            _helper = effectHelper;
            _color = new RgbColor(0, 0.5f, 0.5f);
            _render = _helper.CreateRender();

            _threshold = 0.5f;
            _barSize = (int)(width / NumBars);
        }

        public override IRgbRender Render(float dt)
        {
            _render.Clear();
            var chromas = _helper.GetChromas();

            for (int i = 0; i < NumBars; i++)
                if (chromas[i] >= _threshold)
                {
                    int x0 = i * _barSize;
                    FillBar(x0, _color, (float) chromas[i]);
                }

            return _render;
        }

        private void FillBar(int x0, RgbColor color, float intensity)
        {
            for (int x = x0; x < x0 + _barSize; x++)
                _render[x, 0] = color * intensity;
        }

        public override void GenerateSettings(ILayerSettingsPanel settingsPanel)
        {
            settingsPanel.AddColorGroup(this, _color, (uint)ValueId.Color);

            settingsPanel.GroupBegin("Threshold");
            settingsPanel.AddTrackbar(this, _threshold, 0, 1, (uint)ValueId.ThresholdValue);
            settingsPanel.GroupEnd();
        }

        public override void ValueChanged(uint valueId, object value)
        {
            switch ((ValueId) valueId)
            {
                case ValueId.Color:
                    _color = _helper.ValueToColor(value);
                    break;

                case ValueId.ThresholdValue:
                    _threshold = _helper.ValueToSingle(value);
                    break;
            }
        }

        public string GetName()
        {
            return "Chroma Bars";
        }

        public string GetVersion()
        {
            return "v1.0.0";
        }

        public string GetAuthor()
        {
            return "Audectra";
        }
    }
}

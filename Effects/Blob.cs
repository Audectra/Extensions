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
    class Blob : EffectBase, IExtension
    {
        private IEffectHelper _helper;
        private RgbColor _color;
        private IRgbRender _render;

        private float _x0Pos;
        private float _y0Pos;
        private float _xSize;
        private float _ySize;

        private enum ValueId
        {
            ColorValue = 0,
            XPositionValue,
            YPositionValue,
            XSizeValue,
            YSizeValue
        }

        public Blob() { }

        public Blob(IEffectHelper effectHelper, int height, int width) : base(height, width)
        {
            _helper = effectHelper;
            _color = new RgbColor(0, 0.5f, 0.5f);
            _render = _helper.CreateRender();

            _x0Pos = Width / 2;
            _y0Pos = Height / 2;

            if (Width >= Height && Height > 1)
            {
                _ySize = Height / 2;
                _xSize = _ySize;
            }
            else
            {
                _xSize = Width / 2;
                _ySize = _xSize;
            }
        }

        public override IRgbRender Render(float dt)
        {
            _render.Clear();
            _helper.AddBlob(_render, _x0Pos, _y0Pos, _xSize, _ySize, _color);

            return _render;
        }

        public override void GenerateSettings(ILayerSettingsPanel settingsPanel)
        {
            settingsPanel.AddColorGroup(this, _color, (uint)ValueId.ColorValue);

            settingsPanel.GroupBegin("Position");
            settingsPanel.AddTrackbar(this, _x0Pos, 0, Width, (uint)ValueId.XPositionValue);

            if (Height > 1)
                settingsPanel.AddTrackbar(this, _y0Pos, 0, Height, (uint)ValueId.YPositionValue);

            settingsPanel.GroupEnd();

            settingsPanel.GroupBegin("Size");
            settingsPanel.AddTrackbar(this, _xSize, 0, Width, (uint)ValueId.XSizeValue);

            if (Height > 1)
                settingsPanel.AddTrackbar(this, _ySize, 0, Height, (uint) ValueId.YSizeValue);

            settingsPanel.GroupEnd();
        }

        public override void ValueChanged(uint valueId, object value)
        {
            switch ((ValueId) valueId)
            {
                case ValueId.ColorValue:
                    _color = _helper.ValueToColor(value);
                    break;

                case ValueId.XPositionValue:
                    _x0Pos = _helper.ValueToSingle(value);
                    break;

                case ValueId.YPositionValue:
                    _y0Pos = _helper.ValueToSingle(value);
                    break;

                case ValueId.XSizeValue:
                    _xSize = _helper.ValueToSingle(value);
                    break;

                case ValueId.YSizeValue:
                    _ySize = _helper.ValueToSingle(value);
                    break;
            }
        }

        public string GetName()
        {
            return "Blob";
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

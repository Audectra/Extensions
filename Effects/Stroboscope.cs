/**
 * Copyright (c) Audectra e.U. - All Rights Reserved
 * This effect is part of Audectra.
 */

using System;

using Audectra.Gui;
using Audectra.Graphics;
using Audectra.Graphics.Effects;

namespace Audectra.Extensions.Effects
{
    class Stroboscope : EffectBase, IExtension
    {
        private IEffectHelper _helper;
        private RgbColor _color;
        private IRgbRender _render;

        private float _flashDuration;

        private const float MaxFlashDuration = 0.1f;

        private enum TriggerId
        {
            Flash,
        }

        public Stroboscope() { }

        public Stroboscope(IEffectHelper effectHelper, int height, int width) : base(height, width)
        {
            _helper = effectHelper;
            _color = new RgbColor(0, 0, 0);
            _render = _helper.CreateRender();

            _flashDuration = MaxFlashDuration;
        }

        public override IRgbRender Render(float dt)
        {
            if (_flashDuration > MaxFlashDuration)
                _color = new RgbColor(0, 0, 0);
            else
            {
                _flashDuration += dt;
                _color = _color * (1f - _flashDuration / MaxFlashDuration);
            }

            _render.Map((color, x, y) => _color);
            return _render;
        }

        public override void GenerateSettings(ILayerSettingsPanel settingsPanel)
        {
            settingsPanel.GroupBegin("Trigger");
            settingsPanel.AddBindableTrigger(this, (uint)TriggerId.Flash);
            settingsPanel.GroupEnd();
        }

        public override void ValueChanged(uint valueId, object value)
        {
            
        }

        public override void Trigger(uint triggerId, bool risingEdge)
        {
            switch ((TriggerId) triggerId)
            {
                case TriggerId.Flash:
                    if (risingEdge)
                    {
                        _color = new RgbColor(1, 1, 1);
                        _flashDuration = 0;
                    }
                    break;
            }
        }

        public string GetName()
        {
            return "Stroboscope";
        }

        public string GetVersion()
        {
            return "v1.0.1";
        }

        public string GetAuthor()
        {
            return "Audectra";
        }
    }
}

﻿/**
 * Copyright (c) Audectra e.U. - All Rights Reserved
 * This effect is part of Audectra.
 */

using System;
using System.Runtime.InteropServices;

using Audectra.GUI;
using Audectra.Graphics;
using Audectra.Graphics.Effects;

namespace Audectra.Extensions.Effects
{
    class Bars : EffectBase, IExtension
    {
        private IEffectHelper _helper;
        private RgbColor _color;
        private IRgbRender _render;

        private struct BarItem
        {
            public float Value;
            public float MaxLife;
            public RgbColor Color;
        }

        private int _maxNumBars;
        private float _maxBarLife;
        private int _numBars;
        private int _barSize;
        private int _lastBar;
        private BarItem[] _bars;
        private Random _rand;

        private readonly object _barsLock = new object();

        private enum ValueId
        {
            Color = 0,
            NumBars,
            MaxBarLife,
        }

        private enum TriggerId
        {
            BarBoom = 0,
        }

        public Bars() { }

        public Bars(IEffectHelper effectHelper, int height, int width) : base(height, width)
        {
            _helper = effectHelper;
            _color = new RgbColor(0, 0.5f, 0.5f);
            _render = _helper.CreateRender();

            _maxNumBars = Math.Min((int)(width / 2), 12);
            _numBars = 4;
            _maxBarLife = 1.0f;
            _barSize = (int)(width / _numBars);
            _lastBar = -1;
            _rand = new Random();

            InitBars();
        }

        private void InitBars()
        {
            _bars = new BarItem[_numBars];

            for (int i = 0; i < _numBars; i++)
                _bars[i].Color = new RgbColor(0, 0, 0);
        }

        public override IRgbRender Render(float dt)
        {
            lock (_barsLock)
            {
                UpdateBars(dt);

                for (int i = 0; i < _numBars; i++)
                {
                    int x0 = i * _barSize;
                    float intensity = 0;
                    
                    if (_bars[i].MaxLife > 0)
                        intensity = _bars[i].Value / _bars[i].MaxLife;

                    _helper.FillBar(_render, x0, _barSize, _bars[i].Color * intensity);
                }
            }

            return _render;
        }

        private void UpdateBars(float dt)
        {
            for (int i = 0; i < _numBars; i++)
            {
                _bars[i].Value -= dt;

                if (_bars[i].Value < 0)
                    _bars[i].Value = 0;
            }
        }

        public override void GenerateSettings(ILayerSettingsPanel settingsPanel)
        {
            settingsPanel.AddColorGroup(this, _color, (uint)ValueId.Color);

            settingsPanel.GroupBegin("Num Bars");
            settingsPanel.AddTrackbar(this, _numBars, 1, _maxNumBars, (uint)ValueId.NumBars);
            settingsPanel.GroupEnd();

            settingsPanel.GroupBegin("Bar Life");
            settingsPanel.AddBindableTrackbar(this, _maxBarLife, 0, 2.5, (uint)ValueId.MaxBarLife);
            settingsPanel.GroupEnd();

            settingsPanel.GroupBegin("Trigger");
            settingsPanel.AddBindableTrigger(this, (uint)TriggerId.BarBoom);
            settingsPanel.GroupEnd();
        }

        public override void ValueChanged(uint valueId, object value)
        {
            switch ((ValueId) valueId)
            {
                case ValueId.Color:
                    _color = _helper.ValueToColor(value);
                    break;

                case ValueId.NumBars:
                    lock (_barsLock)
                    {
                        _numBars = _helper.ValueToInt(value);
                        _barSize = (int)(Width / _numBars);
                        _lastBar = -1;
                        InitBars();
                    }
                    break;

                case ValueId.MaxBarLife:
                    lock (_barsLock)
                    {
                        _maxBarLife = _helper.ValueToSingle(value);
                    }
                    break;
            }
        }

        public override void Trigger(uint triggerId, bool enable)
        {
            if (!enable)
                return;

            switch ((TriggerId) triggerId)
            {
                case TriggerId.BarBoom:
                {
                    int idxBar = GetNextBar();

                    lock (_barsLock)
                    {
                        _bars[idxBar].Value = _maxBarLife;
                        _bars[idxBar].MaxLife = _maxBarLife;
                        _bars[idxBar].Color = _color.Copy();
                    }
                }
                break;
            }
        }

        private int GetNextBar()
        {
            int nextBar;

            do
            {
                nextBar = _rand.Next(_numBars);
            } while (nextBar == _lastBar);

            _lastBar = nextBar;
            return nextBar;
        }

        public string GetName()
        {
            return "Bars";
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
using Godot;
using System;
using System.Collections.Generic;

[Tool]
public partial class VolumeSlider : HBoxContainer
{
    private static Dictionary<int, double> _volumePercantageByBusIndex = new Dictionary<int, double>();

    private string? _busName = "<Bus Name>";
    private RichTextLabel? _volumeLabel;
    private HSlider? _volumeSlider;
    private RichTextLabel? _percantageLabel;

    [Export] public string? BusName
    {
        get { return _busName; }
        set { _busName = value; UpdateVolumeLabel(); }
    }

    [Export] public int BusIndex { get; set; }

    public override void _Ready()
    {
        _volumeLabel = GetNode<RichTextLabel>("VolumeLabel");
        UpdateVolumeLabel();

        _volumeSlider = GetNode<HSlider>("VolumeSlider");
        _volumeSlider.ValueChanged += _volumeSlider_ValueChanged;

        _percantageLabel = GetNode<RichTextLabel>("PercantageLabel");
        UpdatePercantageLabel();

        if (_volumePercantageByBusIndex.ContainsKey(BusIndex))
        {
            _volumeSlider.Value = _volumePercantageByBusIndex[BusIndex];
        } else { _volumeSlider.Value = 50.0; }
    }

    private void _volumeSlider_ValueChanged(double value)
    {
        UpdatePercantageLabel();
        _volumePercantageByBusIndex[BusIndex] = value;

        if (value == 0)
        {
            AudioServer.SetBusMute(BusIndex, true); return;
        }

        if (AudioServer.IsBusMute(BusIndex))
        {
            AudioServer.SetBusMute(BusIndex, false);
        }

        float decibels = ConvertPercantageToDb(value);
        AudioServer.SetBusVolumeDb(BusIndex, decibels);
    }

    private void UpdateVolumeLabel()
    {
        if (_volumeLabel == null) { return; }

        _volumeLabel.Text = _busName;
    }

    private void UpdatePercantageLabel()
    {
        double volume = _volumeSlider.Value;
        string volumePercantage = $"{Mathf.Floor(volume)}%";
        _percantageLabel.Text = volumePercantage;
    }

    private float ConvertPercantageToDb(double percantage)
    {
        float scale = 20.0f;
        float divisor = 50.0f;
        return scale * (float)Math.Log10(percantage / divisor);
    }
}

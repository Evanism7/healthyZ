namespace healthyZ.Views;
using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
//using UIKit;

public partial class Turntable : ContentPage
{
    private float _angle = 0;
    private readonly List<string> _segments = new();
    private readonly Color[] _colors = {
    Color.FromArgb("#C1D9C2"), // �ٯ����K��
    Color.FromArgb("#F1E4C4"), // ������
    Color.FromArgb("#DCD6E5"), // ��������
    Color.FromArgb("#EBDDF5"), // ��������
    Color.FromArgb("#F6F3EE"), // ���o�ǥ�
    Color.FromArgb("#EAD9C5"), // �����d��
    Color.FromArgb("#CAD8B4"), // �H���V��
    Color.FromArgb("#D7E3F1")  // ��������
};

    public Turntable()
	{
		InitializeComponent();
        WheelView.Drawable = new WheelDrawable(_segments, _colors, () => _angle);
    }

    //������s
    private async void TurntableClicked(object sender, EventArgs e)
    {
        var random = new Random();
        float target = _angle + 360 * 5 + random.Next(0, 360); // 5��+�H������
        this.Animate("spin", d =>
        {
            _angle = (float)d;
            WheelView.Invalidate();
        }, _angle, target, 16, 2000, Easing.CubicOut);
        _angle = target % 360;
    }

    class WheelDrawable : IDrawable
    {
        private readonly IList<string> _segments;
        private readonly Color[] _colors;
        private readonly Func<float> _getAngle;

        public WheelDrawable(IList<string> segments, Color[] colors, Func<float> getAngle)
        {
            _segments = segments;
            _colors = colors;
            _getAngle = getAngle;
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            float centerX = dirtyRect.Center.X;
            float centerY = dirtyRect.Center.Y;
            float radius = Math.Min(dirtyRect.Width, dirtyRect.Height) / 2 - 10;
            float angle = _getAngle();
            float sweep = 360f / _segments.Count;

            canvas.SaveState();
            canvas.Translate(centerX, centerY);
            canvas.Rotate(angle);

            for (int i = 0; i < _segments.Count; i++)
            {
                canvas.FillColor = _colors[i % _colors.Length];

                // �إ߮��θ��|
                var path = new PathF();
                path.MoveTo(0, 0);

                float startAngle = i * sweep;
                float endAngle = startAngle + sweep;
                int segments = 30; // ���u�����ƫ�
                for (int j = 0; j <= segments; j++)
                {
                    float arcAngle = startAngle + (endAngle - startAngle) * j / segments;
                    float rad = (float)(Math.PI * arcAngle / 180.0);
                    float x = (float)(radius * Math.Cos(rad));
                    float y = (float)(radius * Math.Sin(rad));
                    path.LineTo(x, y);
                }
                path.Close();

                canvas.FillPath(path);

                // �e����r�����ƦC�]�C�Ӧr�@��A�m���^
                canvas.SaveState();
                canvas.Rotate(startAngle + sweep / 2 + 90); // ����q��������
                canvas.FontColor = Color.FromArgb("#48786F");
                canvas.FontSize = 20;

                string text = _segments[i];
                float charHeight = 22; // �C�Ӧr�����Z�A�i�̦r���j�p�L��
                float startY = -radius / 1.8f - (text.Length - 1) * charHeight / 2; // ����q�m��

                for (int c = 0; c < text.Length; c++)
                {
                    canvas.DrawString(text[c].ToString(), 0, startY + c * charHeight, HorizontalAlignment.Center);
                }
                canvas.RestoreState();
            }
            canvas.RestoreState();

            // �e���w�]�b��ߥ��W��A�^��쥻�y�Шt��e�^
            canvas.SaveState();
            canvas.Translate(centerX, centerY);
            canvas.FillColor = Color.FromArgb("#E26A6A");

            float pointerHeight = 30; // ���w����
            float pointerBase = (float)(pointerHeight * Math.Sqrt(3)); // ���T���Ω����
            float offset = 10; // ��~���Z

            // ���I�b��ߥ��W���~
            float tipY = -radius - pointerHeight - offset;
            float baseY = -radius - offset;

            var pointer = new PathF();
            pointer.MoveTo(0, -radius + 10);      // ���I�]��ߥ��W��^
            pointer.LineTo(-10, -radius - 20);    // ���U
            pointer.LineTo(10, -radius - 20);     // �k�U
            pointer.Close();
            canvas.FillPath(pointer);
            canvas.RestoreState();
        }
    }

    private void OnAddClicked(object sender, EventArgs e)
    {
        var text = FoodEditor.Text;
        if (!string.IsNullOrWhiteSpace(text))
        {
            // �H������ΡA�h���ťզ�
            var items = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => s.Trim())
                            .Where(s => !string.IsNullOrEmpty(s));
            foreach (var item in items)
            {
                _segments.Add(item);
            }
            WheelView.Drawable = new WheelDrawable(_segments, _colors, () => _angle);
            WheelView.Invalidate();
        }
    }

    private void OnSpinClicked(object sender, EventArgs e)
    {
        var random = new Random();
        float target = _angle + 360 * 5 + random.Next(0, 360); // 5��+�H������
        this.Animate("spin", d =>
        {
            _angle = (float)d;
            WheelView.Invalidate();
        }, _angle, target, 16, 2000, Easing.CubicOut);
        _angle = target % 360;
    }

    private void OnResetClicked(object sender, EventArgs e)
    {
        _segments.Clear();
        FoodEditor.Text = string.Empty; // �M�Ť�r��
        WheelView.Drawable = new WheelDrawable(_segments, _colors, () => _angle);
        WheelView.Invalidate();
    }
}
using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.FlagMarker
{
    public class CrpgPortcullisVisualWidget
    : Widget
    {
        public CrpgPortcullisVisualWidget(UIContext context)
            : base(context)
        {
            _outlineWidget.Sprite = GetSprite("ui_crpg_mission_marker_portcullis_outline");
            _iconWidget.Sprite = GetSprite("ui_crpg_mission_marker_portcullis");
        }

        protected override void OnLateUpdate(float dt)
        {
            base.OnLateUpdate(dt);
            if (!_hasVisualSet && OutlineWidget != null && IconWidget != null)
            {
                _hasVisualSet = true;
            }
        }

        private Sprite GetSprite(string path)
        {
            return Context.SpriteData.GetSprite(path);
        }

        public Widget OutlineWidget
        {
            get
            {
                return _outlineWidget;
            }
            set
            {
                if (_outlineWidget != value)
                {
                    _outlineWidget = value;
                    OnPropertyChanged<Widget>(value, "OutlineWidget");
                }
            }
        }

        public Widget IconWidget
        {
            get
            {
                return _iconWidget;
            }
            set
            {
                if (_iconWidget != value)
                {
                    _iconWidget = value;
                    OnPropertyChanged<Widget>(value, "IconWidget");
                }
            }
        }

        private bool _hasVisualSet;
        private Widget _outlineWidget = default!;
        private Widget _iconWidget = default!;
    }
}

﻿using PhotoFrame.Logic.BL;
using PhotoFrame.Logic.Config;

namespace PhotoFrame.Logic.UI.ViewModels
{
    public class ViewModelNormal : ViewModelBase
    {
        public ViewModelNormal(IFrameController frameController, IFrameConfig config)
            : base(frameController, config)
        {
        }
    }
}

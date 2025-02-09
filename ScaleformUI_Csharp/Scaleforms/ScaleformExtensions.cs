﻿using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using ScaleformUI.Elements;
using System.Drawing;

namespace ScaleformUI.Scaleforms
{
    public class ScaleformWideScreen : INativeValue, IDisposable
    {
        public ScaleformWideScreen(string scaleformID)
        {
            _handle = API.RequestScaleformMovieInstance(scaleformID);
        }

        ~ScaleformWideScreen()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (IsLoaded)
            {
                API.SetScaleformMovieAsNoLongerNeeded(ref _handle);
            }

            GC.SuppressFinalize(this);
        }


        public int Handle
        {
            get { return _handle; }
        }

        private int _handle;

        public override ulong NativeValue
        {
            get
            {
                return (ulong)Handle;
            }
            set
            {
                _handle = unchecked((int)value);
            }
        }

        public bool IsValid
        {
            get
            {
                return Handle != 0;
            }
        }
        public bool IsLoaded
        {
            get
            {
                return API.HasScaleformMovieLoaded(Handle);
            }
        }

        public void CallFunction(string function, params object[] arguments)
        {
            API.BeginScaleformMovieMethod(Handle, function);
            foreach (object argument in arguments)
            {
                if (argument is int argInt)
                {
                    API.PushScaleformMovieMethodParameterInt(argInt);
                }
                else if (argument is string || argument is char)
                {
                    API.PushScaleformMovieMethodParameterString(argument.ToString());
                }
                else if (argument is double || argument is float)
                {
                    API.PushScaleformMovieMethodParameterFloat((float)argument);
                }
                else if (argument is bool argBool)
                {
                    API.PushScaleformMovieMethodParameterBool(argBool);
                }
                else if (argument is ScaleformLabel argLabel)
                {
                    API.BeginTextCommandScaleformString(argLabel.Label);
                    API.EndTextCommandScaleformString();
                }
                else if (argument is ScaleformLiteralString argLiteral)
                {
                    API.ScaleformMovieMethodAddParamTextureNameString_2(argLiteral.LiteralString);
                }
                else
                {
                    throw new ArgumentException(string.Format("Unknown argument type '{0}' passed to scaleform with handle {1}...", argument.GetType().Name, Handle), "arguments");
                }
            }
            API.EndScaleformMovieMethod();
        }

        public void Render2D()
        {
            API.DrawScaleformMovieFullscreen(Handle, 255, 255, 255, 255, 0);
        }
        public void Render2DScreenSpace(PointF location, PointF size)
        {
            float x = location.X / Screen.Width;
            float y = location.Y / Screen.Height;
            float width = size.X / Screen.Width;
            float height = size.Y / Screen.Height;

            API.DrawScaleformMovie(Handle, x + (width / 2.0f), y + (height / 2.0f), width, height, 255, 255, 255, 255, 0);
        }
        public void Render3D(Vector3 position, Vector3 rotation, Vector3 scale)
        {
            API.DrawScaleformMovie_3dNonAdditive(Handle, position.X, position.Y, position.Z, rotation.X, rotation.Y, rotation.Z, 2.0f, 2.0f, 1.0f, scale.X, scale.Y, scale.Z, 2);
        }
        public void Render3DAdditive(Vector3 position, Vector3 rotation, Vector3 scale)
        {
            API.DrawScaleformMovie_3d(Handle, position.X, position.Y, position.Z, rotation.X, rotation.Y, rotation.Z, 2.0f, 2.0f, 1.0f, scale.X, scale.Y, scale.Z, 2);
        }
    }
}

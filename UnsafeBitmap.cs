﻿/*  AeroShot - Transparent screenshot utility for Windows
	Copyright (C) 2011 Caleb Joseph

	AeroShot is free software: you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation, either version 3 of the License, or
	(at your option) any later version.

	AeroShot is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with this program.  If not, see <http://www.gnu.org/licenses/>. */

using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace AeroShot {
	internal struct PixelData {
		internal byte Red;
		internal byte Green;
		internal byte Blue;
		internal byte Alpha;

		internal PixelData(byte color) {
			Red = color;
			Green = color;
			Blue = color;
			Alpha = 255;
		}
	}

	internal unsafe class UnsafeBitmap {
		private readonly Bitmap _inputBitmap;
		private BitmapData _bitmapData;
		private Byte* _pBase = null;
		private PixelData* _pixelData = null;
		private int _width;

		internal UnsafeBitmap(Bitmap inputBitmap) {
			_inputBitmap = inputBitmap;
		}

		internal void LockImage() {
			var bounds = new Rectangle(Point.Empty, _inputBitmap.Size);

			_width = bounds.Width*sizeof (PixelData);
			if (_width%4 != 0) _width = 4*(_width/4 + 1);

			//Lock Image
			_bitmapData = _inputBitmap.LockBits(bounds, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
			_pBase = (Byte*) _bitmapData.Scan0.ToPointer();
		}

		internal PixelData GetPixel(int x, int y) {
			_pixelData = (PixelData*) (_pBase + y*_width + x*sizeof (PixelData));
			return *_pixelData;
		}

		internal void SetPixel(int x, int y, PixelData p) {
			var data = (PixelData*) (_pBase + y*_width + x*sizeof (PixelData));
			data->Red = p.Red;
			data->Green = p.Green;
			data->Blue = p.Blue;
			data->Alpha = p.Alpha;
		}

		internal void UnlockImage() {
			_inputBitmap.UnlockBits(_bitmapData);
			_bitmapData = null;
			_pBase = null;
		}
	}
}
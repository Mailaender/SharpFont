﻿#region MIT License
/*Copyright (c) 2012 Robert Rouhani <robert.rouhani@gmail.com>

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
of the Software, and to permit persons to whom the Software is furnished to do
so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/
#endregion

using System;
using System.Runtime.InteropServices;

namespace SharpFont
{
	/// <summary>
	/// FreeType root glyph slot class structure. A glyph slot is a container
	/// where individual glyphs can be loaded, be they in outline or bitmap 
	/// format.
	/// </summary>
	/// <remarks>
	/// If FT_Load_Glyph is called with default flags (see FT_LOAD_DEFAULT) the
	/// glyph image is loaded in the glyph slot in its native format (e.g., an
	/// outline glyph for TrueType and Type 1 formats).
	/// 
	/// This image can later be converted into a bitmap by calling 
	/// FT_Render_Glyph. This function finds the current renderer for the
	/// native image's format, then invokes it.
	/// 
	/// The renderer is in charge of transforming the native image through the
	/// slot's face transformation fields, then converting it into a bitmap
	/// that is returned in ‘slot->bitmap’.
	/// 
	/// Note that ‘slot->bitmap_left’ and ‘slot->bitmap_top’ are also used to 
	/// specify the position of the bitmap relative to the current pen position
	/// (e.g., coordinates (0,0) on the baseline). Of course, ‘slot->format’ is
	/// also changed to FT_GLYPH_FORMAT_BITMAP.
	/// </remarks>
	/// <example>
	/// FT_Pos  origin_x	   = 0;
	///	FT_Pos  prev_rsb_delta = 0;
	/// 
	/// 
	///	for all glyphs do
	///	&lt;compute kern between current and previous glyph and add it to
	///		`origin_x'&gt;
	/// 
	///	&lt;load glyph with `FT_Load_Glyph'&gt;
	/// 
	/// if ( prev_rsb_delta - face-&gt;glyph-&gt;lsb_delta &gt;= 32 )
	/// 	origin_x -= 64;
	/// else if ( prev_rsb_delta - face->glyph-&gt;lsb_delta &lt; -32 )
	/// 	origin_x += 64;
	/// 
	/// prev_rsb_delta = face-&gt;glyph->rsb_delta;
	/// 
	/// &lt;save glyph image, or render glyph, or ...&gt;
	/// 
	/// origin_x += face-&gt;glyph-&gt;advance.x;
	/// endfor  
	/// </example>
	public sealed class GlyphSlot
	{
		internal IntPtr reference;

		internal GlyphSlot(IntPtr reference)
		{
			this.reference = reference;
		}

		/// <summary>
		/// A handle to the FreeType library instance this slot belongs to.
		/// </summary>
		public Library Library
		{
			get
			{
				return new Library(Marshal.ReadIntPtr(reference, 0));
			}
		}

		/// <summary>
		/// A handle to the parent face object.
		/// </summary>
		public Face Face
		{
			get
			{
				return new Face(Marshal.ReadIntPtr(reference, 0
					+ IntPtr.Size));
			}
		}

		/// <summary>
		/// In some cases (like some font tools), several glyph slots per face
		/// object can be a good thing. As this is rare, the glyph slots are
		/// listed through a direct, single-linked list using its ‘next’ field.
		/// </summary>
		public GlyphSlot Next
		{
			get
			{
				return new GlyphSlot(Marshal.ReadIntPtr(reference, 0
					+ IntPtr.Size * 2));
			}
		}

		//private uint reserved;

		/// <summary>
		/// A typeless pointer which is unused by the FreeType library or any
		/// of its drivers. It can be used by client applications to link
		/// their own data to each glyph slot object.
		/// </summary>
		public Generic Generic
		{
			get
			{
				//add 4 because of a private "reserved" field between next and generic.
				return new Generic(reference, 4 
					+ IntPtr.Size * 3);
			}
		}

		/// <summary>
		/// The metrics of the last loaded glyph in the slot. The returned
		/// values depend on the last load flags (see the FT_Load_Glyph API
		/// function) and can be expressed either in 26.6 fractional pixels or
		/// font units.
		/// 
		/// Note that even when the glyph image is transformed, the metrics are
		/// not.
		/// </summary>
		public GlyphMetrics Metrics
		{
			get
			{
				return new GlyphMetrics(reference, 4 
					+ IntPtr.Size * 3 
					+ Generic.SizeInBytes);
			}
		}

		/// <summary>
		/// The advance width of the unhinted glyph. Its value is expressed in
		/// 16.16 fractional pixels, unless FT_LOAD_LINEAR_DESIGN is set when
		/// loading the glyph. This field can be important to perform correct
		/// WYSIWYG layout. Only relevant for outline glyphs.
		/// </summary>
		public int LinearHorizontalAdvance
		{
			get
			{
				return Marshal.ReadInt32(reference, 4 
					+ IntPtr.Size * 3 
					+ Generic.SizeInBytes 
					+ GlyphMetrics.SizeInBytes);
			}
		}

		/// <summary>
		/// The advance height of the unhinted glyph. Its value is expressed in
		/// 16.16 fractional pixels, unless FT_LOAD_LINEAR_DESIGN is set when
		/// loading the glyph. This field can be important to perform correct
		/// WYSIWYG layout. Only relevant for outline glyphs.
		/// </summary>
		public int LinearVerticalAdvance
		{
			get
			{
				return Marshal.ReadInt32(reference, 8 
					+ IntPtr.Size * 3 
					+ Generic.SizeInBytes 
					+ GlyphMetrics.SizeInBytes);
			}
		}

		/// <summary>
		/// This shorthand is, depending on FT_LOAD_IGNORE_TRANSFORM, the
		/// transformed advance width for the glyph (in 26.6 fractional pixel
		/// format). As specified with FT_LOAD_VERTICAL_LAYOUT, it uses either
		/// the ‘horiAdvance’ or the ‘vertAdvance’ value of ‘metrics’ field.
		/// </summary>
		public Vector2i Advance
		{
			get
			{
				return new Vector2i(reference, 12 
					+ IntPtr.Size * 3 
					+ Generic.SizeInBytes 
					+ GlyphMetrics.SizeInBytes);
			}
		}

		/// <summary>
		/// This field indicates the format of the image contained in the glyph
		/// slot. Typically FT_GLYPH_FORMAT_BITMAP, FT_GLYPH_FORMAT_OUTLINE, or
		/// FT_GLYPH_FORMAT_COMPOSITE, but others are possible.
		/// </summary>
		public GlyphFormat Format
		{
			get
			{
				return (GlyphFormat)Marshal.ReadInt32(reference, 12 
					+ IntPtr.Size * 3 
					+ Generic.SizeInBytes 
					+ GlyphMetrics.SizeInBytes 
					+ Vector2i.SizeInBytes);
			}
		}

		/// <summary>
		/// This field is used as a bitmap descriptor when the slot format is
		/// FT_GLYPH_FORMAT_BITMAP. Note that the address and content of the
		/// bitmap buffer can change between calls of FT_Load_Glyph and a few
		/// other functions.
		/// </summary>
		public Bitmap Bitmap
		{
			get
			{
				return new Bitmap(reference, 16 
					+ IntPtr.Size * 3 
					+ Generic.SizeInBytes 
					+ GlyphMetrics.SizeInBytes 
					+ Vector2i.SizeInBytes);
			}
		}

		/// <summary>
		/// This is the bitmap's left bearing expressed in integer pixels. Of
		/// course, this is only valid if the format is FT_GLYPH_FORMAT_BITMAP.
		/// </summary>
		public int BitmapLeft
		{
			get
			{
				return Marshal.ReadInt32(reference, 16 
					+ IntPtr.Size * 3 
					+ Generic.SizeInBytes 
					+ GlyphMetrics.SizeInBytes 
					+ Vector2i.SizeInBytes 
					+ Bitmap.SizeInBytes);
			}
		}

		/// <summary>
		/// This is the bitmap's top bearing expressed in integer pixels.
		/// Remember that this is the distance from the baseline to the
		/// top-most glyph scanline, upwards y coordinates being positive.
		/// </summary>
		public int BitmapTop
		{
			get
			{
				return Marshal.ReadInt32(reference, 20 
					+ IntPtr.Size * 3 
					+ Generic.SizeInBytes 
					+ GlyphMetrics.SizeInBytes 
					+ Vector2i.SizeInBytes 
					+ Bitmap.SizeInBytes);
			}
		}

		/// <summary>
		/// The outline descriptor for the current glyph image if its format is
		/// FT_GLYPH_FORMAT_OUTLINE. Once a glyph is loaded, ‘outline’ can be
		/// transformed, distorted, embolded, etc. However, it must not be
		/// freed.
		/// </summary>
		public Outline Outline
		{
			get
			{
				return new Outline(reference, 24 
					+ IntPtr.Size * 3 
					+ Generic.SizeInBytes 
					+ GlyphMetrics.SizeInBytes 
					+ Vector2i.SizeInBytes 
					+ Bitmap.SizeInBytes);
			}
		}

		/// <summary>
		/// The number of subglyphs in a composite glyph. This field is only
		/// valid for the composite glyph format that should normally only be
		/// loaded with the FT_LOAD_NO_RECURSE flag. For now this is internal
		/// to FreeType.
		/// </summary>
		public uint SubglyphsCount
		{
			get
			{
				return (uint)Marshal.ReadInt32(reference, 24 
					+ IntPtr.Size * 3 
					+ Generic.SizeInBytes 
					+ GlyphMetrics.SizeInBytes 
					+ Vector2i.SizeInBytes 
					+ Bitmap.SizeInBytes 
					+ Outline.SizeInBytes);
			}
		}

		/// <summary>
		/// An array of subglyph descriptors for composite glyphs. There are
		/// ‘num_subglyphs’ elements in there. Currently internal to FreeType.
		/// </summary>
		public SubGlyph[] Subglyphs
		{
			get
			{
				int count = (int)SubglyphsCount;

				if (count == 0)
					return null;

				SubGlyph[] subglyphs = new SubGlyph[count];
				IntPtr array = Marshal.ReadIntPtr(reference, 28
					+ IntPtr.Size * 3
					+ Generic.SizeInBytes
					+ GlyphMetrics.SizeInBytes
					+ Vector2i.SizeInBytes
					+ Bitmap.SizeInBytes
					+ Outline.SizeInBytes);

				for (int i = 0; i < count; i++)
				{
					subglyphs[i] = new SubGlyph(array, IntPtr.Size * i);
				}

				return subglyphs;
			}
		}

		/// <summary>
		/// Certain font drivers can also return the control data for a given
		/// glyph image (e.g. TrueType bytecode, Type 1 charstrings, etc.).
		/// This field is a pointer to such data.
		/// </summary>
		public IntPtr ControlData
		{
			get
			{
				return Marshal.ReadIntPtr(reference, 28
					+ IntPtr.Size * 4
					+ Generic.SizeInBytes
					+ GlyphMetrics.SizeInBytes
					+ Vector2i.SizeInBytes
					+ Bitmap.SizeInBytes
					+ Outline.SizeInBytes);
			}
		}

		/// <summary>
		/// This is the length in bytes of the control data.
		/// </summary>
		public int ControlLength
		{
			get
			{
				return Marshal.ReadInt32(reference, 28
					+ IntPtr.Size * 5
					+ Generic.SizeInBytes
					+ GlyphMetrics.SizeInBytes
					+ Vector2i.SizeInBytes
					+ Bitmap.SizeInBytes
					+ Outline.SizeInBytes);
			}
		}

		/// <summary>
		/// The difference between hinted and unhinted left side bearing while
		/// autohinting is active. Zero otherwise.
		/// </summary>
		public int DeltaLSB
		{
			get
			{
				return Marshal.ReadInt32(reference, 32
					+ IntPtr.Size * 5
					+ Generic.SizeInBytes
					+ GlyphMetrics.SizeInBytes
					+ Vector2i.SizeInBytes
					+ Bitmap.SizeInBytes
					+ Outline.SizeInBytes);
			}
		}

		/// <summary>
		/// The difference between hinted and unhinted right side bearing while
		/// autohinting is active. Zero otherwise.
		/// </summary>
		public int DeltaRSB
		{
			get
			{
				return Marshal.ReadInt32(reference, 36
					+ IntPtr.Size * 5
					+ Generic.SizeInBytes
					+ GlyphMetrics.SizeInBytes
					+ Vector2i.SizeInBytes
					+ Bitmap.SizeInBytes
					+ Outline.SizeInBytes);
			}
		}

		/// <summary>
		/// Really wicked formats can use this pointer to present their own
		/// glyph image to client applications. Note that the application needs
		/// to know about the image format.
		/// </summary>
		public IntPtr Other
		{
			get
			{
				return Marshal.ReadIntPtr(reference, 40
					+ IntPtr.Size * 5
					+ Generic.SizeInBytes
					+ GlyphMetrics.SizeInBytes
					+ Vector2i.SizeInBytes
					+ Bitmap.SizeInBytes
					+ Outline.SizeInBytes);
			}
		}
	}
}
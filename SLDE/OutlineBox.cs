using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SLDE
{
	/// <summary>
	/// Controls a rectangular outline with a transparent fill, parented to a Form
	/// </summary>
	public class OutlineBox : Component
	{
		Control top, bottom, left, right;
		Color color = Color.LightBlue;
		Rectangle rectangle;
		int thickness = 5;
		bool visible;

		/// <summary>
		/// The box's color
		/// </summary>
		[Browsable(true)]
		public virtual Color Color
		{
			get { return color; }
			set
			{
				color = value;
				top.BackColor = value;
				bottom.BackColor = value;
				left.BackColor = value;
				right.BackColor = value;
			}
		}

		/// <summary>
		/// The client-space rectangle
		/// </summary>
		[Browsable(true)]
		public virtual Rectangle Rectangle
		{
			get { return rectangle; }
			set
			{
				rectangle = value;
				Refresh();
			}
		}

		/// <summary>
		/// The thickness in pixels
		/// </summary>
		[Browsable(true)]
		public virtual int Thickness
		{
			get { return thickness; }
			set
			{
				thickness = value;
				Refresh();
			}
		}

		/// <summary>
		/// Gets and sets whether the box is visible
		/// </summary>
		[Browsable(true)]
		public virtual bool Visible
		{
			get { return visible; }
			set
			{
				if(visible != value)
				{
					top.Visible = value;
					bottom.Visible = value;
					left.Visible = value;
					right.Visible = value;
					visible = value;
				}
			}
		}

		/// <summary>
		/// Brings the box to the front
		/// </summary>
		public virtual void BringToFront()
		{
			top.BringToFront();
			bottom.BringToFront();
			left.BringToFront();
			right.BringToFront();
		}

		/// <summary>
		/// Refreshes the child controls with respect to the current properties
		/// </summary>
		public virtual void Refresh()
		{
			top.Location = rectangle.Location;
			top.Size = new Size(rectangle.Width, thickness);
			left.Location = rectangle.Location;
			left.Size = new Size(thickness, rectangle.Height);
			right.Location = new Point(rectangle.X + rectangle.Width - thickness,
				rectangle.Y);
			right.Size = left.Size;
			bottom.Location = new Point(rectangle.X,
				rectangle.Y + rectangle.Height - thickness);
			bottom.Size = top.Size;
		}

		/// <summary>
		/// Creates a new instance, parented to the given form
		/// </summary>
		/// <param name="form">The form to parent to</param>
		/// <exception cref="ArgumentNullException"><paramref name="form"/>
		/// is <c>null</c></exception>
		public OutlineBox(Form form)
		{
			if (form == null)
				throw new ArgumentNullException("form");
			form.Controls.Add(top = new Control());
			form.Controls.Add(bottom = new Control());
			form.Controls.Add(left = new Control());
			form.Controls.Add(right = new Control());
			Color = Color;
		}

		static Dictionary<Form, OutlineBox> cache
			= new Dictionary<Form, OutlineBox>();

		/// <summary>
		/// Gets the <see cref="OutlineBox"/> for a given <see cref="Form"/>
		/// </summary>
		/// <param name="form">The parent form</param>
		/// <returns>An OutlineBox instance</returns>
		public static OutlineBox Get(Form form)
		{
			if (form == null)
				throw new ArgumentNullException("form");
			OutlineBox box;
			if(!cache.TryGetValue(form, out box))
			{
				box = new OutlineBox(form);
				cache.Add(form, box);
				form.Disposed += form_Disposed;
			}
			return box;
		}

		static void form_Disposed(object sender, EventArgs e)
		{
			cache.Remove((Form)sender);
		}

	}
}

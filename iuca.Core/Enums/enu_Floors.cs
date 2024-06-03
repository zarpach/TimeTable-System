using System;
using System.ComponentModel.DataAnnotations;

namespace iuca.Application.Enums
{
	public enum enu_Floors
	{
		[Display(Name = "0-ой этаж")]
		GroundFloor = 0,

		[Display(Name = "1-ый этаж")]
		FirstFloor = 1,

		[Display(Name = "2-ой этаж")]
		SecondFloor = 2,

		[Display(Name = "3-ий этаж")]
		ThirdFloor = 3
	}
}


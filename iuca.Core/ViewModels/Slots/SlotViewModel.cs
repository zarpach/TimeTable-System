using System;
using iuca.Application.DTO.Slots;
using System.Collections.Generic;
using iuca.Application.ViewModels.Users.Instructors;
using System.Collections;
using System.Linq;

namespace iuca.Application.ViewModels.Slots
{
    public class SlotViewModel
    {
        public SlotDTO SingleSlot { get; set; }
        public IEnumerable<IGrouping<string, SlotDTO>> AllSlots { get; set; }
        public SlotOptionsViewModel SlotOptionsViewModel = new();
    }
}


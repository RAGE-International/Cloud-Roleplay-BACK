using Backend.Models.ClothingModel.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Models.ClothingModel
{
    public class ClothingModel
    {
        public List<ComponentModel> clothingComponents { get; set; } = new List<ComponentModel>();
    }

    public class ClothingData
    {
    }
}

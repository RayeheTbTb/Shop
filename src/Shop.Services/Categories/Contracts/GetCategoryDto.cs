﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Services.Categories.Contracts
{
    public class GetCategoryDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }
}
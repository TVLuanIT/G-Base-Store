using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GBStore.Data;
using GBStore.Models;

namespace GBStore.Controllers
{
    public class BrandsController : Controller
    {
        private readonly GbstoreContext _context;

        public BrandsController(GbstoreContext context)
        {
            _context = context;
        }
    }
}

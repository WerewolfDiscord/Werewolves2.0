using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web
{
	public class DIdentityDbContext : IdentityDbContext<DIdentityUser, DIdentityRole, string>
	{
		public DIdentityDbContext(DbContextOptions<DIdentityDbContext> options) : base(options)
		{
			//nothing here
		}
	}
}

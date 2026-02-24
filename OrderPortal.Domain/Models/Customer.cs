using System;
using System.Collections.Generic;
using System.Text;

namespace OrderPortal.Domain.Models;

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    public List<Invoice> Invoices { get; set; } = new();
}
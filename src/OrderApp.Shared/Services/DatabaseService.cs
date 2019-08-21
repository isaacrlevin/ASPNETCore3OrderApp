
using LiteDB;
using OrderApp.Shared.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OrderApp.App.Services
{
    public class DatabaseService
    {
        string path = $"{Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)}\\OrderApp\\OrderApp.db";
        string folder = $"{Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)}\\OrderApp\\";

        public DatabaseService()
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
        }

        public void AddOrder(Order order)
        {
            using (var connection = new LiteDatabase(path))
            {
                var orders = connection.GetCollection<Order>();
                orders.Insert(order);
            }
        }

        public List<Order> GetOrders()
        {
            using (var connection = new LiteDatabase(path))
            {
                var orders = connection.GetCollection<Order>();
                var result = orders.FindAll();
                return result.ToList();
            }
        }

        public Order GetPendingOrder()
        {
            using (var connection = new LiteDatabase(path))
            {
                var orders = connection.GetCollection<Order>();
                var result = orders.FindOne(x => x.Status == "Pending");
                return result;
            }
        }

        public void UpdateOrder(int orderId)
        {
            using (var connection = new LiteDatabase(path))
            {
                var orders = connection.GetCollection<Order>();
                var order = orders.FindById(orderId);
                order.Status = "Processed";

                orders.Update(order);
            }
        }
    }
}
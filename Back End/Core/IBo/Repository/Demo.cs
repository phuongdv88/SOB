using MI.Bo.IRepository;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Bo.Repository
{
    public class Demo : IDemo
    {
        private readonly IConfiguration _configuration;
        private readonly string _connStr;
        private readonly IExecuters _executers;
        public Demo(IConfiguration configuration, IExecuters executers)
        {
            _configuration = configuration;
            _connStr = _configuration.GetConnectionString("DefaultConnection");
            _executers = executers;
        }
        public string GetDemo()
        {
            //Khai báo biến
            //var p = new DynamicParameters();
            //Name Store 
            //var commandText = "usp_Web_GetArticleById";
            //Excute Query
            //var result = _executers.ExecuteCommand(_connStr, conn => conn.QueryFirstOrDefault<ArticleDetail>(commandText, p, commandType: System.Data.CommandType.StoredProcedure));
            return "";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sjbgWebService
{

    public class CjcxData
    {
        public string plan_date { get; set; }
        public string dd { get; set; }
        public string work_no { get; set; }
        public string work_name { get; set; }
        public string cjjg { get; set; }
    }
    public class DcjhData
    {
        public string plan_date { get; set; }
        public string dri_Room_no { get; set; }
        public string Drive_name { get; set; }
        public string Dri_time { get; set; }
        public string Ass_name { get; set; }
        public string Ass_time { get; set; }
        public string Student_name { get; set; }
        public string Stu_time { get; set; }

    }
    public class JcjhData
    {
        public string plan_date { get; set; }
        public string open_time { get; set; }
        public string locus { get; set; }
        public string engi_brand { get; set; }
        public string engi_no { get; set; }

    }

    public class RyjhData
    {
        public string plan_date { get; set; }
        public string locus { get; set; }
        public string engi_no { get; set; }
        public string Roadway { get; set; }
        public string ZunDian_time { get; set; }
        public string driver_1no { get; set; }
        public string driver_2no { get; set; }
        public string driver_3no { get; set; }

    }

    public class MingPai
    {
        public string GongHao { get; set; }
        public string XingMing { get; set; }
        public string XianBieID { get; set; }
        public string XianBie { get; set; }
        public string ZhuangTai { get; set; }
        public int BanCi { get; set; }
        public int WeiZhi { get; set; }
    }

    public class DaMingPai
    {
        public string XianBie { get; set; }
        public string XianBieID { get; set; }
        public int BanCi { get; set; }
        public string GongHao1 { get; set; }
        public string XingMing1 { get; set; }
        public string ZhuangTai1 { get; set; }
        public string GongHao2 { get; set; }
        public string XingMing2 { get; set; }
        public string ZhuangTai2 { get; set; }
        public string GongHao3 { get; set; }
        public string XingMing3 { get; set; }
        public string ZhuangTai3 { get; set; }
        public string GongHao4 { get; set; }
        public string XingMing4 { get; set; }
        public string ZhuangTai4 { get; set; }
    }

    public class XianBie
    {
        public string XianBieID { get; set; }
        public string XianBieMingCheng { get; set; }
        public int LeiXing { get; set; }
    }

    public class JiCheJiHua
    {
        public string XianBieID { get; set; }
        public string XianBie { get; set; }
        public string JiChe { get; set; }
        public string CheCi { get; set; }
        public string KaiDian { get; set; }
        public string DiDian { get; set; }
    }

    public class ChuTuiQin
    {
        public string GongHao1 { get; set; }
        public string XingMing1 { get; set; }
        public string GongHao2 { get; set; }
        public string XingMing2 { get; set; }
        public string XianBieID { get; set; }
        public string XianBie { get; set; }
        public string KaiDaoDian { get; set; }
        public string ChuTuiQinDian { get; set; }
        public string JiChe { get; set; }
        public string CheCi { get; set; }
    }

    public class ChuChengJiLu
    {
        public string XianBie { get; set; }
        public string XianBieID { get; set; }
        
        public string GongHao1 { get; set; }
        public string XingMing1 { get; set; }
        public string GongHao2 { get; set; }
        public string XingMing2 { get; set; }
        public string GongHao3 { get; set; }
        public string XingMing3 { get; set; }
        public string GongHao4 { get; set; }
        public string XingMing4 { get; set; }
    }

    public class CanBu
    {
        public string GongHao { get; set; }
        public string XingMing { get; set; }
        public string CheCi { get; set; }
        public string KaiChe { get; set; }
        public string DaoDa { get; set; }
        public string LingQV { get; set; }
        public string JinE { get; set; }
        public string ShiChang { get; set; }
    }
}
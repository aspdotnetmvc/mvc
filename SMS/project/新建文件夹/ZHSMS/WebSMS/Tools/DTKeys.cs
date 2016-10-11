using System;
using System.Collections.Generic;
using System.Text;


public class DTKeys
{
    //系统版本
    /// <summary>
    /// 版本号全称
    /// </summary>
    public const string ASSEMBLY_VERSION = "1.0.3";
    /// <summary>
    /// 版本年号
    /// </summary>
    public const string ASSEMBLY_YEAR = "2012";
    //File======================================================
    /// <summary>
    /// 插件配制文件名
    /// </summary>
    public const string FILE_PLUGIN_XML_CONFING = "plugin.config";
    /// <summary>
    /// 站点配置文件名
    /// </summary>
    public const string FILE_SITE_XML_CONFING = "Configpath";
    /// <summary>
    /// URL配置文件名
    /// </summary>
    public const string FILE_URL_XML_CONFING = "Urlspath";
    /// <summary>
    /// 升级代码
    /// </summary>
    public const string FILE_URL_UPGRADE_CODE = "267C2643EE401DD2F0A06084F7931C4DEC76E7CAA1996481FE8F5081A8936409058D07A6F5E2941C";
    /// <summary>
    /// 消息代码
    /// </summary>
    public const string FILE_URL_NOTICE_CODE = "267C2643EE401DD2F0A06084F7931C4DEC76E7CAA1996481FE8F5081A8936409D037BEA6A623A0A1";

    //Directory==================================================
    /// <summary>
    /// ASPX目录
    /// </summary>
    public const string DIRECTORY_REWRITE_ASPX = "aspx";
    /// <summary>
    /// HTML目录
    /// </summary>
    public const string DIRECTORY_REWRITE_HTML = "html";

    //Cache======================================================
    /// <summary>
    /// 站点配置
    /// </summary>
    public const string CACHE_SITE_CONFIG = "dt_cache_site_config";
    /// <summary>
    /// 客户端站点配置
    /// </summary>
    public const string CACHE_SITE_CONFIG_CLIENT = "dt_cache_site_client_config";
    /// <summary>
    /// HttpModule映射类
    /// </summary>
    public const string CACHE_SITE_HTTP_MODULE = "dt_cache_http_module";
    /// <summary>
    /// URL重写映射表
    /// </summary>
    public const string CACHE_SITE_URLS = "dt_cache_site_urls";
    /// <summary>
    /// 升级通知
    /// </summary>
    public const string CACHE_OFFICIAL_UPGRADE = "dt_official_upgrade";
    /// <summary>
    /// 官方消息
    /// </summary>
    public const string CACHE_OFFICIAL_NOTICE = "dt_official_notice";

    //Session=====================================================
    /// <summary>
    /// 验证码
    /// </summary>
    public const string SESSION_CODE = "dt_session_code";
    /// <summary>
    /// 后台管理员
    /// </summary>
    public const string SESSION_ADMIN_INFO = "dt_session_admin_info";

    //Cookies=====================================================
    /// <summary>
    /// 防重复顶踩KEY
    /// </summary>
    public const string COOKIE_DIGG_KEY = "dt_cookie_digg_key";
    /// <summary>
    /// 防重复评论KEY
    /// </summary>
    public const string COOKIE_COMMENT_KEY = "dt_cookie_comment_key";
}

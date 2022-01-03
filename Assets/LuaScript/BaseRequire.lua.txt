
Mathf		= require "Common.UnityEngine.Mathf"
Vector2		= require "Common.UnityEngine.Vector2"
Vector3 	= require "Common.UnityEngine.Vector3"
Vector4		= require "Common.UnityEngine.Vector4"
Quaternion	= require "Common.UnityEngine.Quaternion"
Color		= require "Common.UnityEngine.Color"
Ray			= require "Common.UnityEngine.Ray"
Bounds		= require "Common.UnityEngine.Bounds"
RaycastHit	= require "Common.UnityEngine.RaycastHit"
Touch		= require "Common.UnityEngine.Touch"
LayerMask	= require "Common.UnityEngine.LayerMask"
Plane		= require "Common.UnityEngine.Plane"
Time		= require "Common.UnityEngine.Time"
Object		= require "Common.UnityEngine.Object"

NetMgr = CS.XLuaDemo.NetworkManager.Instance

require("Common.BaseClass")

require("Common.Singleton")
require("Common.Util.List")
require("Common.Util.Dictionary")




--游戏模块
require("Game.Common.UpdateManager")



--UI模块
require("Game.UI.UIManager")

require("Game.Common.UIGlobal")

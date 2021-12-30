--[[
-- OOKAMI
-- 脚本由生成器生成
-- 2021/12/30 19:30:03
-- 请勿更改
-- 请勿更改
-- 请勿更改 
]]

---@class LoginView : BasePanel
local LoginView = BaseClass(BasePanel)
function LoginView:Constructor()
	self.conf = {
		prefabPath = "Assets/AssetBundleRes/UI/LoginPanel.prefab";
		root ="Low";
	}
end

function LoginView:InitUIComponent(root)
	
	self.rImgImage = root:Find("RImg_Image/"):GetComponent("UnityEngine.UI.RawImage")
	self.objEnter = root:Find("Obj_Enter/").gameObject
	self.btnLogin = root:Find("Btn_Login/"):GetComponent("UnityEngine.UI.Button")
	self.txtLoginTxt = root:Find("Btn_Login/Txt_loginTxt/"):GetComponent("UnityEngine.UI.Text")

	
end
return LoginView

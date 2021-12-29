---@class LoginView : BasePanel
---
local LoginView = BaseClass(BasePanel)

function LoginView:Constructor()
    self.conf = {
        prefabPath = "Assets/AssetBundleRes/UI/LoginPanel.prefab";
        root = "High";
    }

end

function LoginView:InitUIComponent(root)
    self.objEnter = root:Find("Obj_Enter").gameObject
    self.enterImage = root:Find("Obj_Enter/Image"):GetComponent("Image")
    self.btnLogin = root:Find("Btn_Login"):GetComponent("Button")
end

return LoginView

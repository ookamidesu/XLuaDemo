
---@class UIManager : Singleton
UIManager = BaseClass(Singleton)

function UIManager:Constructor()
    
end

function UIManager:Init()
    self.canvas = {}
    self.uiRoot = GameObject.Find("UIRoot")
    local canvas = self.uiRoot.transform:Find("Canvas")
    self.hideNode = self.uiRoot.transform:Find("HideUI")
    self.canvas.Low = canvas:Find("Low")
    self.canvas.Normal = canvas:Find("Normal")
    self.canvas.Top = canvas:Find("Top")
    self.canvas.High = canvas:Find("High")
end

---CreatePanel 创建Panel
---@param panel BasePanel
function UIManager:CreatePanel(panel)
    local root = panel.conf.root;
    panel:Create(self.canvas[root],self.OnPanelLoadFinish)
end

function UIManager:OnPanelLoadFinish()
    print("加载完成")
end

---CreatePanel 创建Panel
---@param panel BasePanel
function UIManager:Show(panel)
--[[    local root = panel.conf.root;
    panel:Create(self.canvas[root],self.OnPanelLoadFinish)]]
end


---CreatePanel 创建Panel
---@param panel BasePanel
function UIManager:Hide(panel)
--[[    local root = panel.conf.root;
    panel:Create(self.canvas[root],self.OnPanelLoadFinish)]]
    panel.gameObject:SetActive(false)
    panel.transform:SetParent(self.hideNode)
end


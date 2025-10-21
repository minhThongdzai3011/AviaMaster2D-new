# GMSoft SDK Changelog System

Custom Unity Editor Window hiển thị changelog từ GMSoft SDK branch `gmsoft-sdk-v7` sử dụng **ScriptableObject** để quản lý data.

## ✨ Features

- 🎯 **ScriptableObject-based**: Data quản lý qua Unity Inspector
- 🚀 **Auto-show**: Tự động hiện lần đầu import
- 🎨 **Beautiful UI**: Modern gradient design với styled components
- 📱 **Responsive**: Adaptive layout cho mọi screen size
- 📝 **Simple Editing**: Direct editing through Unity Inspector
- ⭐ **Latest Version Highlight**: Latest version được highlight đặc biệt

## 🛠️ System Architecture

```
ChangelogScriptableObject.asset (Data Storage)
    ↓
ChangelogWindow.cs (Display UI)
    ↓
GMSoftSDKInitializer.cs (Auto-show Logic)
```

## 📁 File Structure

```
Assets/GMSoft/Editor/
├── ChangelogScriptableObject.cs    # Main ScriptableObject
├── ChangelogWindow.cs              # Editor Window
├── ChangelogData.cs                # Data models
├── GMSoftSDKInitializer.cs         # Auto-initialization
├── GMSoft_Changelog.asset          # Sample data
└── README.md                       # Documentation
```

##  Documentation

Xem `CHANGELOG_SYSTEM_DOCS.md` để biết thêm chi tiết về:
- Cấu trúc code
- API documentation  
- Customization guide
- Troubleshooting

## 🔧 Configuration

**Asset Path**: Create changelog asset in any location in your project
- The system will auto-detect ChangelogScriptableObject assets
- Recommended location: `Assets/GMSoft/Editor/GMSoft_Changelog.asset`

# URL Router v1.2 Release Notes

**Release Date:** November 2025

## 🎉 What's New

### Router Executable Icon
- The router executable (`UrlRouter.exe`) now displays the application icon, making it more recognizable and trustworthy when setting it as your default browser
- No more generic .NET icon - your router looks professional!

### Default Browser Status Indicator
- New status panel at the top of the UI shows whether URL Router is set as your default browser
- **Green checkmark** ✓ indicates URL Router is properly configured as default
- **Orange warning** ⚠ with helpful button when not set as default
- Status automatically refreshes when you return to the application

### One-Click Settings Access
- "Set as Default" button opens Windows Settings directly to the default apps configuration
- Step-by-step instructions guide you through the setup process
- No more hunting through Windows Settings menus!

## 🐛 Bug Fixes

### Browser Display Issues
- **Fixed:** Browser name now displays correctly in the rules list (was showing "Default" even when Edge/Chrome was configured)
- Browser dropdown now properly restores selection when editing existing rules
- Browser property is now correctly saved and loaded from configuration

### Rule Processing
- **Fixed:** Router now respects the enabled/disabled flag on rules
- Disabled rules are properly skipped during URL matching
- UI shows disabled rules with visual indicators (grayed out)

### Microsoft Teams Safelink Support
- **Fixed:** URLs from Microsoft Teams (and other Microsoft safelink wrappers) now work correctly
- Router automatically extracts the real URL from Teams safelink wrappers
- Your clickup.com links (and other sites) will now route correctly even when coming from Teams

### Browser Launching
- **Fixed:** Browsers now launch correctly (was failing silently in some cases)
- Improved error handling and validation before launching applications
- Executable paths are now validated before attempting to launch

## 🔧 Improvements

### Enhanced Logging
- Router now logs all activity to `%APPDATA%\UrlRouter\router.log`
- Logs include:
  - When router is called and with what URL
  - Which rules are being checked
  - Which rule matched (if any)
  - What command is being executed
  - Any errors that occur
- Great for debugging routing issues!

### Better User Experience
- Browser dropdown automatically selects matching browser when editing rules
- Improved error messages and user feedback
- Status panel makes it clear when action is needed

### Code Quality
- Better error handling throughout the application
- Improved code organization and maintainability
- Added validation for file paths and configurations

## 📋 Migration Notes

### Upgrading from v1.1 or earlier
- **Automatic:** Your existing configuration will work without changes
- The new "enabled" flag defaults to `true` for all existing rules
- No action required - just install and go!

### Configuration Changes
- Configuration format remains compatible
- New `enabled` field is optional (defaults to `true` if not present)
- Existing configurations will continue to work

## 🔍 Technical Details

### New Components
- `DefaultBrowserHelper.cs` - Handles default browser detection and Windows Settings integration
- Router icon support added to project configuration

### Updated Components
- Router now checks for `enabled` flag in rules
- Router extracts real URLs from Microsoft safelinks
- UI status panel with automatic refresh
- Enhanced rule matching logic

## 📦 Installation

1. Download `UrlRouter-Setup-1.2.exe` from the releases page
2. Run the installer (no admin privileges required)
3. Open the UI application
4. Click "Set as Default" button in the status panel if needed
5. Configure your routing rules

## 🐛 Known Issues

None at this time. If you encounter any issues, please check the log file at:
`%APPDATA%\UrlRouter\router.log`

## 🙏 Thank You

Thank you for using URL Router! If you encounter any issues or have suggestions, please report them on GitHub.

---

**Full Changelog:** See commit history for detailed code changes.




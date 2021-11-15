**SetCodeHeaders** is a simple Visual Studio 2022 extension that does one thing and one thing alone: add text from a "*.sln.headertext" file to ALL of the .CS, .XML, .CONFIG, .XSD and .XAML files in a given solution.  The hand-edited **.sln.headertext file** most typically contains license and author info, but any content that is suitable to be added to the top of your code files is also suitable.

The extension was written as a stop-gap while waiting for Rubicon's superb <a href="https://github.com/rubicon-oss/LicenseHeaderManager" target="_blank">License Header Manager</a> to be updated to work with VS2022, and should not be used as a long-term replacement, since:

* SetCodeHeaders does not support project-specific headers
* SetCodeHeaders does not remove header-text (although it does overwrite)
* SetCodeHeaders does not include localization support
* SetCodeHeaders does not have expandable properties
* SetCodeHeaders has not been well-tested (caveat emptor)

To install the extension, download the SetCodeHeaders project from GitHub, compile a Release version, and then execute the **SetCodeHeaders.vsix** file generated in the bin/Release folder.  Once installed, a "Set Code Headers" menu item will appear on your VS2022 "Tools" menu.  *(NOTE: given the transient nature of this extension, there's no plan to publish it to the Visual Studio Extensions Gallery.)*

As a final preparatory step, you'll need to create a text file in your solution folder, named <SolutionName>.sln.headertext.  The file should contain your header-text.  *NOTE: do not include comment signifiers in your header text (i.e. "//", "<!\--" and/or "-->") as those signifiers will be automatically added for you.*

To update your code files, click on the "Tools/Set Code Headers" menu item. If a file has an existing header, it will be replaced.  Closed files will be permanently updated with the new header-text; open files will be updated in place, with undo-buffer properly set.

Again, this extension has only been minimally tested, so please use it at your own risk.  A good suggestion would be to **only use the extension on projects that are under source control**, and preferably after a recent check in.

Enjoy!

Oh; one more thing.  If you find a bug, please email <a href="mailto:louis@squideyes.com" target="_blank">louis@suideyes.com</a>.  Better yet, offer up a fix via a pull-request.  The source code can be found at <a href="https://github.com/squideyes/SetCodeHeaders" target="_blank">https://github.com/squideyes/SetCodeHeaders</a>.



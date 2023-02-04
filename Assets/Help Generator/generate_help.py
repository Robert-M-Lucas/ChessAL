from importlib.resources import path
import os
import shutil

PATH_TO_HELP = "../StreamingAssets/Help"
PATH_TO_STATIC = "data/static"
PATH_TO_DATA = "data"
TEMPLATE_FILE = "data/template.html"

# Read template file
with open(TEMPLATE_FILE, 'r') as f:
    template_data = f.read()

# Remove all existing files
if os.path.exists(PATH_TO_HELP):
    shutil.rmtree(PATH_TO_HELP)

# Create help directory
os.mkdir(PATH_TO_HELP)

# Copy static files (css, imgs, etc)
shutil.copytree(PATH_TO_STATIC, PATH_TO_HELP + "/" + PATH_TO_STATIC.split("/")[-1])

# Get list of html files
files_in_data = [f for f in os.listdir(PATH_TO_DATA) if os.path.isfile(os.path.join(PATH_TO_DATA, f)) and (f[-5:] == ".html")]
                                                                                       
for file in files_in_data:
    if file == "template.html": continue

    # Read file and add template
    with open(PATH_TO_DATA + "/" + file, "r") as f:
        new_data = template_data.replace("$$$$", f.read())

    # Write file
    with open(PATH_TO_HELP + "/" + file, "w+") as f:
        f.write(new_data)
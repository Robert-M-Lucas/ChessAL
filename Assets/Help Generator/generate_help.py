from importlib.resources import path
import os
import shutil

PATH_TO_HELP = "../StreamingAssets/Help"
PATH_TO_STATIC = "data/static"
PATH_TO_DATA = "data"
TEMPLATE_FILE = "data/template.html"

with open(TEMPLATE_FILE, 'r') as f:
    template_data = f.read()

if os.path.exists(PATH_TO_HELP):
    shutil.rmtree(PATH_TO_HELP)

os.mkdir(PATH_TO_HELP)

shutil.copytree(PATH_TO_STATIC, PATH_TO_HELP + "/" + PATH_TO_STATIC.split("/")[-1])

files_in_data = [f for f in os.listdir(PATH_TO_DATA) if os.path.isfile(os.path.join(PATH_TO_DATA, f))]

for file in files_in_data:
    if file == "template.html": continue

    with open(PATH_TO_DATA + "/" + file, "r") as f:
        new_data = template_data.replace("$$$$", f.read())

    with open(PATH_TO_HELP + "/" + file, "w+") as f:
        f.write(new_data)
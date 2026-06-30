import urllib.request, json
url = "https://api.modrinth.com/v2/project/9eGKb6K1/version?game_versions=[%221.21.1%22]"
req = urllib.request.Request(url, headers={'User-Agent': 'Mozilla/5.0'})
data = json.loads(urllib.request.urlopen(req).read())
valid = [v for v in data if 'fabric' in v['loaders']]
for v in valid[:10]:
    print(f"{v['version_number']} -> {v['files'][0]['url']}")

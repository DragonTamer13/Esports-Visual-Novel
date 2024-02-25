import csv;
import os;

ChatFileRelativePath = "\..\..\Resources\LivestreamMessages.csv"
Emotes = ["LUL", "OMEGALUL", "4Head", "ABOBA", "POG", "POGGERS", "widepepOMEGAKEKHappyChampHands"]

if __name__ == "__main__":
    print("Hello world")

    dirname = os.path.dirname(__file__)
    dirname = dirname.replace("Scripts\HelperScripts", "Resources\LivestreamMessages.csv")
    write_dirname = dirname.replace("Messages", "Messages_TMP")
    print(dirname)
    headers = []
    content = []

    with open(dirname, newline='', encoding='utf8') as chatFile:
        reader = csv.reader(chatFile, delimiter=",", quotechar='"')
        headers = next(reader)
        for row in reader:
            new_message = ""
            for word in row[1].split(" "):
                if word in Emotes:
                    new_message += '<sprite="' + word + '" name="' + word + '"> '
                else:
                    new_message += word + " "

            # -1 to trim the last space
            content.append([row[0], new_message[:len(new_message)-1]])

    with open(dirname, 'w', encoding='utf8', newline='') as write_csv:
        writer = csv.writer(write_csv)

        writer.writerow(headers)
        writer.writerows(content)
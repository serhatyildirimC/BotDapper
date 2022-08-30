const telegramBot = require('node-telegram-bot-api');
const token = "5428730965:AAFrJ4zAZ82miL2qR-90lVc7KRDmhhWCYio"
const bot = new telegramBot(token, { polling: true })
const fetch = require('isomorphic-unfetch')

bot.on('message', async(msg) => {
    const chatId = msg.chat.id;
    const massage = msg.text.trim().toLowerCase();
    switch (massage) {
        case 'haber':
            {
                bot.sendMessage(chatId, "bekle")
                const result = await getNews(1)
                Array.from(Array(5)).forEach((i, index) => {
                    bot.sendPhoto(chatId, result[index].urlToImage, { caption: `${result[index].title}/n ${result[index].description}` })

                })
            } //console.log("debug : " + result[0].title);

            break;

        case 'resim':
            bot.sendPhoto(chatId, "https://i.ytimg.com/vi/FulyL4oFj40/maxresdefault.jpg")
            break;


        case 'olur':
            bot.sendMessage(chatId, "olmaz")
            break;
        case 'serkan':
            bot.sendMessage(chatId, "did u mean jöle")
            break;
        case 'acan':
            bot.sendMessage(chatId, "did u mean yavşak")
            break;
        case 'neden':
            bot.sendMessage(chatId, "ne neden ?")
            break;


        default:
            bot.sendMessage(chatId, "seçeneklerde yok")
            break;
    }
})
const getNews = (number) => {
    return fetch("https://newsapi.org/v2/top-headlines?sources=techcrunch&apiKey=2ced73a733d2411bb3468ec4e9bf7853")
        .then(response => response.json())
        .then(response => {
            return response.articles
        })
}
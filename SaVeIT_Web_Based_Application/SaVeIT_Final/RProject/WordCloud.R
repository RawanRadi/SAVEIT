if (!require(RODBC)) {
    install.packages("RODBC")
    library(RODBC)
}
if (!require(SnowballC)) {
    install.packages("SnowballC")
    library(SnowballC)
}
if (!require(tm)) {
    install.packages("tm")
    library(tm)
}
if (!require(wordcloud)) {
    install.packages("wordcloud")
    library(wordcloud)
}
if (!require(RColorBrewer)) {
    install.packages("RColorBrewer")
    library(RColorBrewer)
}

cn <- odbcDriverConnect('Driver={SQL Server};Server=(local);Database=SaVaITFinal-DB;Trusted_Connection=yes')
df <- sqlQuery(cn, "select SPAbstract from SeniorProject;")
odbcClose(cn);

textex <- df$SPAbstract
ExCorpus <- Corpus(VectorSource(textex))
inspect(ExCorpus)

ExCorpus <- tm_map(ExCorpus, content_transformer(tolower))
ExCorpus <- tm_map(ExCorpus, removePunctuation)
ExCorpus <- tm_map(ExCorpus, removeWords, stopwords('english'))
ExCorpus <- tm_map(ExCorpus, stripWhitespace)

ExCorpus <- tm_map(ExCorpus, removeWords, c("will", "also", "one", "two", "using", "use", "can", "need"))



wd <- getwd()
imagedir <- paste("C:\\Users\\COMNET\\source\\repos\\SaVeIT_Project\\SaVeIT_Final\\SaVeIT_Final\\Content\\html\\", sep = "")
setwd(imagedir)
filename <- 'WordCloud.jpeg'
if (file.exists(filename))
    file.remove(filename)
jpeg("C:\\Users\\COMNET\\source\\repos\\SaVeIT_Project\\SaVeIT_Final\\SaVeIT_Final\\Content\\html\\WordCloud.jpeg", width = 8, height = 4, units = 'in', res = 300)
wordcloud(ExCorpus, min.freq = 1, max.words = 200, random.order = FALSE, rot.per = 0.35, colors = brewer.pal(8, "Dark2"));
dev.off()

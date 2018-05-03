if (!require(tidyverse)) {
    install.packages("tidyverse")
    library(tidyverse)
}
if (!require(RODBC)) {
    install.packages("RODBC")
    library(RODBC)
}
dbConnection <- 'Driver={SQL Server};Server=(local);Database=SaVaITFinal-DB;Trusted_Connection=yes'
cn <- odbcDriverConnect(dbConnection)
df <- sqlQuery(cn, "select [Year],[progLang] = B.RetVal From  SeniorProject A Cross Apply [dbo].[udf-Str-Parse](A.[progLang],',') B where progLang IS NOT NULL;")
odbcClose(cn);
# This counts the occurrences of each PL-year combo and fills in 
# zeros for the combos that didn't appear.
df_counts <- df %>% group_by(progLang, Year) %>% tally() %>% ungroup() %>% complete(progLang, Year) %>% replace_na(list(n = 0))
# This uses the transformed data, with year on x, n (count) on y, 
# and each PL getting a different color.
q <- ggplot(df_counts, aes(Year, n, color = progLang))
q <- q + labs(x = "Years")
q <- q + geom_line(size = 1)
q <- q + scale_x_continuous(minor_breaks = NULL)
q <- q + scale_color_discrete(name = "Programming Languages")
wd <- getwd()
imagedir <- paste("C:\\Users\\Robiah-PC\\Desktop\\SaVeIT_Final(1)\\SaVeIT_Final\\SaVeIT_Final\\Content\\html\\", sep = "")
setwd(imagedir)
filename <- 'PL_Trend.png'
if (file.exists(filename))
    file.remove(filename)
ggsave(q, file = filename, width = 8, height = 4)
setwd(wd)
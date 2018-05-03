if (!require(RODBC)) {
    install.packages("RODBC")
    library(RODBC)
}
if (!require(ggplot2)) {
    install.packages("ggplot2")
    library(ggplot2)
}
if (!require(forcats)) {
    install.packages("forcats")
    library(forcats)
}
if (!require(R2HTML)) {
    install.packages("R2HTML")
    library(R2HTML)
}
cn <- odbcDriverConnect('Driver={SQL Server};Server=(local);Database=SaVaITFinal-DB;Trusted_Connection=yes')
df <- sqlQuery(cn, "select AOIName from AreaOFInterest a, ProjectsAOI p where p.AOIId = a.AOIId;")
odbcClose(cn)
jpeg("C:\\Users\\COMNET\\source\\repos\\SaVeIT_Project\\SaVeIT_Final\\SaVeIT_Final\\Content\\html\\WordCld.jpeg", width = 5, height = 5, units = 'in', res = 300)

q <- ggplot(df, aes(fct_infreq(AOIName)))
q <- q + labs(x = "Area of Interest")
q <- q + geom_bar(position = position_stack(reverse = TRUE), fill = "#000035")
q <- q + coord_flip()
q <- q + theme(legend.position = "top")

wd <- getwd()
imagedir <- paste("C:\\Users\\COMNET\\source\\repos\\SaVeIT_Project\\SaVeIT_Final\\SaVeIT_Final\\Content\\html\\", sep = "")
setwd(imagedir)
filename <- 'WordFrequencyAOI.png'
if (file.exists(filename))
    file.remove(filename)
ggsave(q, file = filename, width = 8, height = 4)
setwd(wd)
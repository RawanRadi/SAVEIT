if (!require(RODBC)) {
    install.packages("RODBC")
    library(RODBC)
}
if (!require(GGally)) {
    install.packages("GGally")
    library(GGally)
}
if (!require(RColorBrewer)) {
    install.packages("RColorBrewer")
    library(RColorBrewer)
}
if (!require(scales)) {
    install.packages("scales")
    library(scales)
}
if (!require(igraph)) {
    install.packages("igraph")
    library(igraph)
}

cn <- odbcDriverConnect('Driver={SQL Server};Server=(local);Database=SaVaITFinal-DB;Trusted_Connection=yes')
df <- sqlQuery(cn, "select s.SPtId, a.AOIName from ProjectsAOI s, AreaOFInterest a where a.AOIId = s.AOIId ORDER By SPtId ASC;")

Vi <- crossprod(table(df[1:2]))
g <- graph.adjacency(Vi, weighted = TRUE, mode = 'undirected')
g <- simplify(g)


g <- E(g)$weight


wd <- getwd()
imagedir <- paste("C:\\Users\\COMNET\\source\\repos\\SaVeIT_Project\\SaVeIT_Final\\SaVeIT_Final\\Content\\html", sep = "")
setwd(imagedir)
filename <- 'Network.jpeg'
if (file.exists(filename))
    file.remove(filename)
jpeg("C:\Users\COMNET\source\repos\SaVeIT_Project\SaVeIT_Final\SaVeIT_Final\Content\html\\Network.jpeg", width = 6, height = 5, units = 'in', res = 300)
ggnet2(g, size = 11, label = TRUE, label.size = 4.5, edge.size = E(g)$weight, node.size = 6)

dev.off()
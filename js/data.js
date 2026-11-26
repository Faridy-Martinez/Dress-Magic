// Datos iniciales del prototipo. Los recursos gráficos son provisionales.
window.GAME_DATA = {
  scenes: [
    {id:"hielo",name:"Pista de hielo",icon:"⛸️",subtitle:"Brillos y diversión invernal",background:"linear-gradient(145deg,#c8edff,#e9e7ff)",color:"#c8edff"},
    {id:"parque",name:"Parque de juegos",icon:"🎡",subtitle:"Un día lleno de aventuras",background:"linear-gradient(145deg,#d5f5d7,#fff0c9)",color:"#d5f5d7"},
    {id:"cine",name:"Cine de estrellas",icon:"🎬",subtitle:"Luces, cámara y glamour",background:"linear-gradient(145deg,#d9cafa,#f8d9ec)",color:"#d9cafa"},
    {id:"pasteleria",name:"Dulce pastelería",icon:"🧁",subtitle:"Un look delicioso y colorido",background:"linear-gradient(145deg,#ffe0e8,#fff0c8)",color:"#ffe0e8"},
    {id:"baile",name:"Pista de baile fiesta",icon:"🪩",subtitle:"Que comience la música",background:"linear-gradient(145deg,#d9d0ff,#f7d5f0)",color:"#d9d0ff"},
    {id:"concierto",name:"Escenario de concierto",icon:"🎤",subtitle:"¡Brilla sobre el escenario!",background:"linear-gradient(145deg,#f8d1df,#d9d8ff)",color:"#f8d1df"}
  ],
  categories: {
    ropa:[
      {id:"top-rosa",name:"Blusa rosa",icon:"👚",color:"#ed9ac2",shape:"blouse"},
      {id:"vestido-lila",name:"Vestido lila",icon:"👗",color:"#b79ae8",shape:"dress"},
      {id:"vestido-azul",name:"Vestido de hielo",icon:"🩵",color:"#91cbe8",shape:"dress"},
      {id:"falda",name:"Falda brillante",icon:"✨",color:"#e8a8cf",shape:"skirt"},
      {id:"chaqueta",name:"Chaqueta chic",icon:"🧥",color:"#d9a6a0",shape:"jacket"}
    ],
    zapatos:[
      {id:"tacones",name:"Tacones",icon:"👠",color:"#d78bb6"},
      {id:"botas",name:"Botas",icon:"👢",color:"#a58bc9"},
      {id:"patines",name:"Patines",icon:"⛸️",color:"#88cde6"},
      {id:"tenis",name:"Tenis",icon:"👟",color:"#f0a5a5"}
    ],
    peinados:[
      {id:"cabello-cafe",name:"Castaño",icon:"🤎",color:"#654238"},
      {id:"cabello-rubio",name:"Rubio",icon:"🌼",color:"#d8ad55"},
      {id:"cabello-rosa",name:"Rosa mágico",icon:"🌸",color:"#df8db9"},
      {id:"cabello-morado",name:"Violeta",icon:"💜",color:"#8c6aaf"}
    ],
    accesorios:[
      {id:"corona",name:"Tiara",icon:"👑"},
      {id:"estrella",name:"Estrella",icon:"⭐"},
      {id:"lentes",name:"Lentes",icon:"🕶️"},
      {id:"bolso",name:"Bolso",icon:"👜"},
      {id:"moño",name:"Moño",icon:"🎀"},
      {id:"microfono",name:"Micrófono",icon:"🎤"}
    ]
  },
  magicCombos:[
    {id:"magia-hielo",requires:["vestido-azul","patines"],reward:{id:"capa-cristal",name:"Capa de cristal",icon:"🪽",category:"Prenda mágica"},message:"¡Magia helada! Desbloqueaste una capa de cristal."},
    {id:"magia-estrellas",requires:["vestido-lila","estrella"],reward:{id:"aureola-estelar",name:"Aureola estelar",icon:"🌟",category:"Accesorio mágico"},message:"¡Brillaste como una estrella! Desbloqueaste una aureola."},
    {id:"magia-concierto",requires:["top-rosa","microfono"],reward:{id:"microfono-dorado",name:"Micrófono dorado",icon:"🎙️",category:"Accesorio mágico"},message:"¡Eres una estrella del pop! Desbloqueaste un micrófono dorado."}
  ]
};
(() => {
  const data = window.GAME_DATA;
  const $ = (id) => document.getElementById(id);
  const state = {
    scene:null, category:"ropa",
    outfit:{ropa:null,zapatos:null,peinados:null,accesorios:null},
    unlocked:JSON.parse(localStorage.getItem("dressMagicUnlocked")||"[]"),
    discovered:JSON.parse(localStorage.getItem("dressMagicDiscovered")||"[]"),
    sound:true
  };
  const screens=["homeScreen","scenesScreen","dressScreen","collectionScreen","finalScreen"];
  const allItems=Object.values(data.categories).flat();
  let toastTimer;

  function showScreen(id){
    screens.forEach(screen=>$(screen).classList.toggle("active",screen===id));
    window.scrollTo({top:0,behavior:"smooth"});
  }
  function toast(message){
    $("toast").textContent=message;$("toast").classList.add("show");
    clearTimeout(toastTimer);toastTimer=setTimeout(()=>$("toast").classList.remove("show"),2300);
  }
  function saveProgress(){
    localStorage.setItem("dressMagicUnlocked",JSON.stringify(state.unlocked));
    localStorage.setItem("dressMagicDiscovered",JSON.stringify(state.discovered));
    $("collectionCount").textContent=state.unlocked.length;
  }
  function renderScenes(){
    $("sceneGrid").innerHTML=data.scenes.map(scene=>`
      <button class="scene-card" data-scene="${scene.id}" type="button">
        <div class="scene-art" style="background:${scene.background}"><span>${scene.icon}</span></div>
        <div class="scene-info"><strong>${scene.name}</strong><small>${scene.subtitle}</small></div>
      </button>`).join("");
    document.querySelectorAll("[data-scene]").forEach(button=>button.addEventListener("click",()=>startScene(button.dataset.scene)));
  }
  function startScene(id){
    state.scene=data.scenes.find(scene=>scene.id===id);
    state.outfit={ropa:null,zapatos:null,peinados:null,accesorios:null};
    $("sceneTitle").textContent=state.scene.name;$("stageLabel").textContent=state.scene.name;
    $("dollStage").style.background=state.scene.background;
    renderItems();renderLook();applyOutfit();showScreen("dressScreen");
  }
  function renderItems(){
    document.querySelectorAll(".category-tab").forEach(tab=>tab.classList.toggle("active",tab.dataset.category===state.category));
    const items=data.categories[state.category];
    $("itemGrid").innerHTML=items.map(item=>{
      const selected=state.outfit[state.category]?.id===item.id;
      const locked=state.unlocked.some(x=>x.id===item.id);
      return `<button class="item-card ${selected?"selected":""}" data-item="${item.id}" type="button">
        <span class="item-icon">${item.icon}</span><small>${item.name}</small>${locked?'<small> ✨</small>':""}
      </button>`;
    }).join("");
    document.querySelectorAll("[data-item]").forEach(button=>button.addEventListener("click",()=>selectItem(button.dataset.item)));
  }
  function selectItem(id){
    const item=allItems.find(entry=>entry.id===id);
    const category=Object.keys(data.categories).find(key=>data.categories[key].some(entry=>entry.id===id));
    if(!item||!category)return;
    state.outfit[category]=state.outfit[category]?.id===id?null:item;
    renderItems();renderLook();applyOutfit();checkMagic();
  }
  function renderLook(){
    const rows=[["ropa","👗","Ropa"],["zapatos","👟","Zapatos"],["peinados","💇","Cabello"],["accesorios","🎀","Accesorios"]];
    $("lookList").innerHTML=rows.map(([key,icon,label])=>{
      const item=state.outfit[key];
      return `<div class="look-row"><span>${item?item.icon:icon}</span><div><b>${label}</b><br><small>${item?item.name:"Sin seleccionar"}</small></div></div>`;
    }).join("");
  }
  function applyOutfit(){
    const clothing=state.outfit.ropa;
    const shape=$("outfitShape"),detail=$("outfitDetail");
    const shapes={
      dress:"M96 160 Q130 174 164 160 L190 316 Q130 339 70 316Z",
      blouse:"M96 160 Q130 174 164 160 L176 285 Q130 298 84 285Z",
      skirt:"M96 160 Q130 174 164 160 L190 316 Q130 339 70 316Z",
      jacket:"M96 160 Q130 174 164 160 L184 286 Q130 310 76 286Z"
    };
    shape.setAttribute("d",clothing?(shapes[clothing.shape]||shapes.dress):shapes.dress);
    shape.setAttribute("fill",clothing?.color||"#f1a7c8");
    detail.setAttribute("stroke",clothing?.shape==="jacket"?"#fff":"#fff4fa");
    const shoe=state.outfit.zapatos;
    $("shoeLeft").setAttribute("fill",shoe?.color||"#d78bb6");
    $("shoeRight").setAttribute("fill",shoe?.color||"#d78bb6");
    const hair=state.outfit.peinados;
    const hairColor=hair?.color||"#654238";
    $("hairBack").setAttribute("fill",hairColor);$("hairFront").querySelector("path").setAttribute("fill",hairColor);
    const accessory=state.outfit.accesorios;
    const extra=accessory?({corona:'<text x="130" y="22" text-anchor="middle" font-size="30">👑</text>',estrella:'<text x="181" y="130" font-size="28">⭐</text>',lentes:'<text x="130" y="97" text-anchor="middle" font-size="26">🕶️</text>',bolso:'<text x="198" y="270" font-size="28">👜</text>',moño:'<text x="172" y="42" font-size="28">🎀</text>',microfono:'<text x="197" y="250" font-size="27">🎤</text>'}[accessory.id]||""):"";
    $("accessoryLayer").innerHTML=extra;
  }
  function checkMagic(){
    const selected=Object.values(state.outfit).filter(Boolean).map(item=>item.id);
    for(const combo of data.magicCombos){
      if(combo.requires.every(id=>selected.includes(id))&&!state.discovered.includes(combo.id)){
        state.discovered.push(combo.id);state.unlocked.push(combo.reward);saveProgress();
        $("magicMessage").textContent="✦ "+combo.message+" ✦";
        $("magicMessage").classList.remove("show");void $("magicMessage").offsetWidth;$("magicMessage").classList.add("show");
        toast("¡Nueva pieza desbloqueada! "+combo.reward.icon);
      }
    }
  }
  function renderCollection(){
    const items=state.unlocked;
    $("collectionGrid").innerHTML=items.length?items.map(item=>`<div class="collection-item"><span>${item.icon}</span><b>${item.name}</b><small>${item.category}</small></div>`).join(""):'<div class="collection-empty">Todavía no tienes piezas mágicas.<br>¡Prueba combinaciones en el vestidor para descubrirlas! ✨</div>';
  }
  function finishLook(){
    $("finalPreview").textContent=state.outfit.ropa?.icon||"👗";
    $("finalPreview").textContent+="  "+(state.outfit.zapatos?.icon||"")+(state.outfit.accesorios?.icon||"");
    showScreen("finalScreen");
  }

  $("playButton").addEventListener("click",()=>showScreen("scenesScreen"));
  $("collectionButton").addEventListener("click",()=>{renderCollection();showScreen("collectionScreen")});
  $("categoryTabs").addEventListener("click",event=>{
    const tab=event.target.closest("[data-category]");if(!tab)return;
    state.category=tab.dataset.category;renderItems();
  });
  $("clearButton").addEventListener("click",()=>{
    state.outfit={ropa:null,zapatos:null,peinados:null,accesorios:null};renderItems();renderLook();applyOutfit();toast("Vestidor listo para crear otro look ♡");
  });
  $("finishButton").addEventListener("click",finishLook);
  $("againButton").addEventListener("click",()=>showScreen("scenesScreen"));
  $("homeButton").addEventListener("click",()=>showScreen("homeScreen"));
  $("soundButton").addEventListener("click",()=>{state.sound=!state.sound;$("soundButton").textContent=state.sound?"♫":"♪";toast(state.sound?"Sonido activado":"Sonido desactivado")});
  document.querySelectorAll("[data-go]").forEach(button=>button.addEventListener("click",()=>showScreen(button.dataset.go)));

  $("collectionCount").textContent=state.unlocked.length;
  renderScenes();
})();